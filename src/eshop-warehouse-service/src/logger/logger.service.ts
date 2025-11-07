import { Injectable, LoggerService as NestLoggerService } from '@nestjs/common';
import { logs } from '@opentelemetry/api-logs';

@Injectable()
export class LoggerService implements NestLoggerService {
  private otelLogger = logs.getLogger('default');

  log(message: string, context?: string) {
    this.printToConsole('INFO', message, context);
    this.otelLogger.emit({
      body: message,
      severityNumber: 12, // INFO
      severityText: 'INFO',
      attributes: { context },
    });
  }

  error(message: string, trace?: string, context?: string) {
    this.printToConsole('ERROR', message, context, trace);
    this.otelLogger.emit({
      body: message,
      severityNumber: 22, // ERROR
      severityText: 'ERROR',
      attributes: { trace, context },
    });
  }

  warn(message: string, context?: string) {
    this.printToConsole('WARN', message, context);
    this.otelLogger.emit({
      body: message,
      severityNumber: 14, // WARN
      severityText: 'WARN',
      attributes: { context },
    });
  }

  debug(message: string, context?: string) {
    this.printToConsole('DEBUG', message, context);
    this.otelLogger.emit({
      body: message,
      severityNumber: 5, // DEBUG
      severityText: 'DEBUG',
      attributes: { context },
    });
  }

  verbose(message: string, context?: string) {
    this.printToConsole('VERBOSE', message, context);
    this.otelLogger.emit({
      body: message,
      severityNumber: 1, // TRACE/VERBOSE
      severityText: 'VERBOSE',
      attributes: { context },
    });
  }

  private printToConsole(
    level: string,
    message: string,
    context?: string,
    trace?: string,
  ) {
    const timestamp = new Date().toISOString();
    const ctx = context ? `[${context}]` : '';
    const traceText = trace ? `\nTrace: ${trace}` : '';
    console.log(`[${timestamp}] ${level} ${ctx} ${message}${traceText}`);
  }
}
