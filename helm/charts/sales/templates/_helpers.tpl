{{- define "sales.fullname" -}}
{{- include "sales.name" . }}
{{- end -}}

{{- define "sales.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}
