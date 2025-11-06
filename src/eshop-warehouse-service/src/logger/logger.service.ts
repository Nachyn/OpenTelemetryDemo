import { Injectable, Logger } from '@nestjs/common';
import { logs } from '@opentelemetry/api-logs';

@Injectable()
export class LoggerService extends Logger {
  private logger = logs.getLogger('default');

  log(message: string, context?: string) {
    super.log(message, context);
    this.logger.emit({
      body: message,
      severityNumber: 12, // INFO
      severityText: 'INFO',
      attributes: { context },
    });
  }

  error(message: string, trace?: string, context?: string) {
    super.error(message, trace, context);
    this.logger.emit({
      body: message,
      severityNumber: 22, // ERROR
      severityText: 'ERROR',
      attributes: { trace, context },
    });
  }

  // Аналогично можно реализовать warn, debug, verbose
}
