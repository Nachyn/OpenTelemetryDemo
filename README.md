# 🛒 EShop — Observability Stack (.NET/JS OpenTelemetry)

## 📘 Overview

This project demonstrates a **observability setup**. All telemetry data — **logs, metrics, and traces** — is collected through the **OpenTelemetry Collector** and visualized in **Grafana Dashboards**.

**This is a PLAYGROUND setup only - NOT PRODUCTION READY**

## 🧩 Scheme

```
[OrderService / WarehouseService / NotificationService]
            ↓
        [OTel Collector]
            ↓
 [Jaeger / Grafana Tempo]   →  Traces
 [Prometheus]               →  Metrics
 [Grafana Loki]             →  Logs
            ↓
       [Grafana Dashboards]
```

### 🔧 Configured Components

| Component | Description |
|------------|--------------|
| **OpenTelemetry Collector** | Receives, processes, and exports telemetry data |
| **Jaeger / Grafana Tempo** | Distributed tracing backend |
| **Prometheus** | Metrics collection and storage |
| **Grafana Loki** | Log aggregation system |
| **Grafana Dashboards** | Visualization platform for logs, metrics, and traces |

---

## Getting Started

### 1️⃣ Run Required Services

```bash
docker compose up -d
```

This command starts:
- OpenTelemetry Collector  
- Jaeger / Grafana Tempo  
- Prometheus  
- Grafana Loki  
- Grafana  

---

### 2️⃣ Start .NET Services

Open the main solution:

```bash
cd src
open EShop.sln
```

Run the following services:
- `EShop.OrderService`
- `EShop.NotificationService`

---

### 3️⃣ Start Node.js Service

```bash
cd src/eshop-warehouse-service
npm install
npm run start
```

---

## 📊 Visualization

After all services are running:

- **Call OrderService API**
```bash
curl 'http://localhost:5127/api/orders?productId=1&quantity=1' \
  -X 'POST' \
  -H 'Content-Type: application/json'
```
- **Grafana**: [http://localhost:3000](http://localhost:3000)  
  Use the dashboards to explore **logs**, **metrics**, and **traces**.

- **Jaeger / Tempo**: [http://localhost:16686](http://localhost:16686)  
  Visualize distributed traces.

