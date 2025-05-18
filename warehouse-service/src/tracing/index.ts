// tracing.ts
import { NodeSDK } from '@opentelemetry/sdk-node';
import { NestInstrumentation } from '@opentelemetry/instrumentation-nestjs-core';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-http';
import { PeriodicExportingMetricReader } from '@opentelemetry/sdk-metrics';
import { Resource } from '@opentelemetry/resources';
import { SemanticResourceAttributes } from '@opentelemetry/semantic-conventions';
import {
  LoggerProvider,
  BatchLogRecordProcessor,
} from '@opentelemetry/sdk-logs';
import { OTLPLogExporter } from '@opentelemetry/transport-logs-otlp-http';

const traceExporter = new OTLPTraceExporter({
  url: 'http://localhost:4318/v1/traces',
});

const metricExporter = new OTLPMetricExporter({
  url: 'http://localhost:4318/v1/metrics',
});

const logExporter = new OTLPLogExporter({
  url: 'http://localhost:4318/v1/logs',
});

const resource = new Resource({
  [SemanticResourceAttributes.SERVICE_NAME]: 'nestjs-app',
});

const sdk = new NodeSDK({
  resource,
  traceExporter,
  metricReader: new PeriodicExportingMetricReader({
    exporter: metricExporter,
    exportIntervalMillis: 1000,
  }),
  instrumentations: [getNodeAutoInstrumentations(), new NestInstrumentation()],
});

// Logging (optional but useful!)
const loggerProvider = new LoggerProvider({ resource });
loggerProvider.addLogRecordProcessor(new BatchLogRecordProcessor(logExporter));
loggerProvider.forceFlush(); // optional: force flush logs on exit

sdk.start();
