import { NodeSDK } from '@opentelemetry/sdk-node';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { PeriodicExportingMetricReader } from '@opentelemetry/sdk-metrics';
import { NestInstrumentation } from '@opentelemetry/instrumentation-nestjs-core';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-grpc';
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-grpc';
import { OTLPLogExporter } from '@opentelemetry/exporter-logs-otlp-grpc';
import { SEMRESATTRS_SERVICE_NAME } from '@opentelemetry/semantic-conventions';
import { resourceFromAttributes } from '@opentelemetry/resources';
import { logs } from '@opentelemetry/api-logs';

import {
  BatchLogRecordProcessor,
  LoggerProvider,
} from '@opentelemetry/sdk-logs';

const traceExporter = new OTLPTraceExporter({
  url: 'http://localhost:4317',
});

const metricExporter = new OTLPMetricExporter({
  url: 'http://localhost:4317',
});

const logExporter = new OTLPLogExporter({
  url: 'http://localhost:4317',
});

const resource = resourceFromAttributes({
  EnvNameTest: 'EShop',
  [SEMRESATTRS_SERVICE_NAME]: 'EShop.WarehouseService',
  // [ SEMRESATTRS_SERVICE_NAMESPACE ]: "yourNameSpace",
  // [ SEMRESATTRS_SERVICE_VERSION ]: "1.0",
  // [ SEMRESATTRS_SERVICE_INSTANCE_ID ]: "my-instance-id-1",
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

const loggerProvider = new LoggerProvider({
  resource,
  processors: [new BatchLogRecordProcessor(logExporter)],
});
logs.setGlobalLoggerProvider(loggerProvider);

export function startOtelSDK() {
  sdk.start();
  process.on('SIGTERM', () => {
    sdk
      .shutdown()
      .then(() => console.log('Tracing terminated'))
      .catch((error) => console.log('Error terminating tracing', error))
      .finally(() => process.exit(0));
  });
}
