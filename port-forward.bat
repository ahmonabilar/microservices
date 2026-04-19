@echo off
echo Starting port forwarding...
echo   SQL Server : localhost:1433 -> service/sqlserver:1433
echo   RabbitMQ   : localhost:5672 -> service/rabbitmq:5672
echo   RabbitMQ UI: localhost:15672 -> service/rabbitmq:15672
echo   Identity   : https://microservices.identity.local -> ingress (enable minikube ingress addon)
echo.

start "kubectl port-forward sqlserver" cmd /k kubectl port-forward -n microservices service/sqlserver 1433:1433
start "kubectl port-forward rabbitmq" cmd /k kubectl port-forward -n microservices service/rabbitmq 5672:5672 15672:15672

echo Port forwarding started. Close the opened windows to stop.
echo.
echo NOTE: For identity service, ensure:
echo   1. minikube addons enable ingress
echo   2. Add "127.0.0.1 microservices.identity.local" to C:\Windows\System32\drivers\etc\hosts
echo   3. Run: minikube tunnel
