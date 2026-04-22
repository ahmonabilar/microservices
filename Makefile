PROFILE = abilar
HELM_RELEASE = microservices
HELM_CHART = helm

NAMESPACE = microservices

.PHONY: docker-purge wsl-compact minikube-start ingress-enable build-images build-identity build-sales build-gateway deploy deploy-all import-db publish-db

## Remove all Docker containers, images, volumes, networks, configs, secrets, and build cache, then compact WSL disk
docker-purge:
	@echo === Stopping all running containers ===
	-docker stop $(shell docker ps -aq)
	@echo === Removing all containers ===
	-docker rm -f $(shell docker ps -aq)
	@echo === Removing all images ===
	-docker rmi -f $(shell docker images -aq)
	@echo === Removing all volumes ===
	-docker volume rm -f $(shell docker volume ls -q)
	@echo === Removing all networks ===
	-docker network rm $(shell docker network ls -q --filter type=custom)
	@echo === Removing all configs ===
	-docker config rm $(shell docker config ls -q)
	@echo === Removing all secrets ===
	-docker secret rm $(shell docker secret ls -q)
	@echo === Running system prune (containers, images, networks, build cache) ===
	-docker system prune -a -f --volumes
	@echo === Removing build cache ===
	-docker builder prune -a -f
	@echo === Docker purge complete ===

## Compact the WSL2 virtual disk to reclaim space
wsl-compact:
	@echo === Shutting down WSL ===
	wsl.exe --shutdown
	@echo === Compacting WSL disk (run this target from Windows PowerShell/CMD, not from WSL) ===
	@echo Run the following in an elevated PowerShell:
	@echo   wsl --manage docker-desktop --compact
	@echo   wsl --manage docker-desktop-data --compact
	@echo Or if using older Windows, use diskpart:
	@echo   powershell -Command "Optimize-VHD -Path $$env:LOCALAPPDATA\\Docker\\wsl\\disk\\docker_data.vhdx -Mode Full"

## Start Minikube with the abilar profile
minikube-start:
	minikube start -p $(PROFILE)
	@echo === Minikube started with profile $(PROFILE) ===

## Enable the NGINX ingress addon and remove any stale admission webhook
ingress-enable:
	minikube addons enable ingress -p $(PROFILE)
	-kubectl --context $(PROFILE) delete validatingwebhookconfiguration ingress-nginx-admission 2>/dev/null
	@echo === Ingress addon enabled ===

## Build the identity Docker image inside Minikube
build-identity:
	minikube -p $(PROFILE) image build -t identity:latest -f services/identity/src/microservices.identity.OpenIddict/Dockerfile .

## Build the sales Docker image inside Minikube
build-sales:
	minikube -p $(PROFILE) image build -t sales:latest -f services/sales/src/sales/Dockerfile .

## Build the gateway Docker image inside Minikube
build-gateway:
	minikube -p $(PROFILE) image build -t gateway:latest -f app/blazorapp.gateway/Dockerfile .

## Build all application Docker images inside Minikube
build-images: build-identity build-sales build-gateway
	@echo === All images built ===

## Deploy (or upgrade) microservices using Helm
deploy:
	helm upgrade --install $(HELM_RELEASE) $(HELM_CHART) -f $(HELM_CHART)/values.yaml --kube-context $(PROFILE) --namespace $(NAMESPACE) --create-namespace

## Full pipeline: start Minikube, build images, deploy with Helm
deploy-all: minikube-start ingress-enable build-images deploy
	@echo === Microservices deployed to Minikube ($(PROFILE)) ===

## Import mydb.bacpac into SQL Server running in the cluster
import-db:
	@echo === Importing mydb.bacpac into SQL Server ===
	-kubectl --context $(PROFILE) -n $(NAMESPACE) delete pod import-bacpac 2>/dev/null
	kubectl --context $(PROFILE) -n $(NAMESPACE) run import-bacpac \
		--image=mcr.microsoft.com/dotnet/sdk:10.0 \
		--restart=Never \
		--command -- sleep 3600
	kubectl --context $(PROFILE) -n $(NAMESPACE) wait --for=condition=Ready pod/import-bacpac --timeout=120s
	kubectl --context $(PROFILE) -n $(NAMESPACE) exec import-bacpac -- \
		dotnet tool install -g microsoft.sqlpackage
	kubectl --context $(PROFILE) -n $(NAMESPACE) cp backup/mydb.bacpac import-bacpac:/tmp/mydb.bacpac
	kubectl --context $(PROFILE) -n $(NAMESPACE) exec import-bacpac -- \
		/root/.dotnet/tools/sqlpackage \
		/Action:Import \
		/SourceFile:/tmp/mydb.bacpac \
		/TargetServerName:sqlserver,1433 \
		/TargetDatabaseName:AdventureWorksLT2022 \
		/TargetUser:sa \
		"/TargetPassword:P@ssw0rd1234" \
		/TargetTrustServerCertificate:True
	kubectl --context $(PROFILE) -n $(NAMESPACE) delete pod import-bacpac
	@echo === Database import complete ===

MSBUILD = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"

## Build the sales.database DACPAC locally and publish it to SQL Server in the cluster
publish-db:
	@echo === Building DACPAC locally ===
	$(MSBUILD) services/sales/src/sales.database/sales.database.sqlproj /p:Configuration=Release
	@echo === Publishing sales.database to SQL Server ===
	-kubectl --context $(PROFILE) -n $(NAMESPACE) delete pod publish-dacpac --wait 2>/dev/null
	kubectl --context $(PROFILE) -n $(NAMESPACE) run publish-dacpac \
		--image=mcr.microsoft.com/dotnet/sdk:10.0 \
		--restart=Never \
		--command -- sleep 3600
	kubectl --context $(PROFILE) -n $(NAMESPACE) wait --for=condition=Ready pod/publish-dacpac --timeout=120s
	kubectl --context $(PROFILE) -n $(NAMESPACE) exec publish-dacpac -- \
		dotnet tool install -g microsoft.sqlpackage
	kubectl --context $(PROFILE) -n $(NAMESPACE) cp services/sales/src/sales.database/bin/Release/sales.database.dacpac publish-dacpac:/tmp/sales.database.dacpac
	kubectl --context $(PROFILE) -n $(NAMESPACE) exec publish-dacpac -- \
		/root/.dotnet/tools/sqlpackage \
		/Action:Publish \
		/SourceFile:/tmp/sales.database.dacpac \
		/TargetServerName:sqlserver,1433 \
		/TargetDatabaseName:AdventureWorksLT2022 \
		/TargetUser:sa \
		"/TargetPassword:P@ssw0rd1234" \
		/TargetTrustServerCertificate:True
	kubectl --context $(PROFILE) -n $(NAMESPACE) delete pod publish-dacpac
	@echo === Database publish complete ===
