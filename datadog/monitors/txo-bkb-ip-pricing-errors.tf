# Datadog log monitor: TxO BKB IP Pricing - Multibet 403 and 5xx errors
# Container: bb-uat-uks-txo-bkb-ip-pricing
# Log format: Txo Multibet price request failed, {Legs} code: {statusCode} message: {message}

resource "datadog_monitor" "txo_bkb_ip_pricing_multibet_errors" {
  name    = "[UAT] TxO BKB IP Pricing - Multibet 403 and 5xx errors"
  type    = "log alert"
  message = <<-EOT
    TxO Multibet price request failures (403 or 5xx) detected in bb-uat-uks-txo-bkb-ip-pricing.

    Log format: Txo Multibet price request failed, {Legs} code: {statusCode} message: {message}

    @slack-ops
  EOT

  query = <<-EOT
    container_name:bb-uat-uks-txo-bkb-ip-pricing "Txo Multibet price request failed" ("code: 403" OR "code: 5")
  EOT

  tags = ["env:uat", "service:txo-bkb-ip-pricing", "team:txo"]

  # Alert when at least 1 matching log in the evaluation window
  monitor_thresholds {
    critical = 1
    warning  = 0
  }

  # Evaluation window: 5 minutes
  monitor_threshold_windows {
    trigger_window = "last_5m"
    recovery_window = "last_5m"
  }

  enable_logs_sample = true
  include_tags      = true
  priority          = 3
}
