{{- define "identity.fullname" -}}
{{- include "identity.name" . }}
{{- end -}}

{{- define "identity.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}
