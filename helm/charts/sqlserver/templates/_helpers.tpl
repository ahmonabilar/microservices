{{- define "sqlserver.fullname" -}}
{{- include "sqlserver.name" . }}
{{- end -}}

{{- define "sqlserver.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}
