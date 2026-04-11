{{- define "rabbitmq.fullname" -}}
{{- include "rabbitmq.name" . }}
{{- end -}}

{{- define "rabbitmq.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}
