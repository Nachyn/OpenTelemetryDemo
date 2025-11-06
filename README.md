[OrderService / WarehouseService / NotificationService] → [OTel Collector] → [Jaeger/Grafana Tempo] & [Prometheus] & [Grafana Loki] → [Grafana Dashboards]

### Configured Components
*OpenTelemetry Collector* - Receives, processes and exports telemetry data

*Jaeger/Grafana Tempo* - Distributed tracing backend

*Prometheus* - Metrics collection and storage

*Grafana Loki* - Log aggregation system

*Grafana Dashboards* - Visualization platform with csutom dashboards


**Presentation.pptx/pdf in /docs**


1. Run required services 
- `docker compose up -d`
2. Open src/EShop.sln
- run `EShop.OrderService` with port 5001
- run `EShop.Notification`
4. Run eshop-warehouse-service
- `cd src/eshop-warehouse-service`
- `npm install`
- `npm run start`