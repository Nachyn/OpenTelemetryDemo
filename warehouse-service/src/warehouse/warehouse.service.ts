import { Injectable } from '@nestjs/common';
import { SpanStatusCode, trace } from '@opentelemetry/api';
import { OrderItem } from './dto';
import { LoggerService } from '../logger';

@Injectable()
export class WarehouseService {
  private readonly tracer = trace.getTracer('warehouse-service');

  constructor(private readonly loggerService: LoggerService) {}

  private readonly products = [
    { id: 1, name: 'Keyboard', price: 50 },
    { id: 2, name: 'Mouse', price: 30 },
    { id: 3, name: 'Monitor', price: 200 },
    { id: 4, name: 'USB Hub', price: 25 },
    { id: 5, name: 'Desk Lamp', price: 40 },
    { id: 6, name: 'Webcam', price: 90 },
    { id: 7, name: 'Microphone', price: 60 },
    { id: 8, name: 'Chair', price: 130 },
    { id: 9, name: 'Laptop Stand', price: 75 },
    { id: 10, name: 'Notebook Cooler', price: 35 },
  ];

  reserveProduct(productId: number, quantity: number): OrderItem {
    const span = this.tracer.startSpan('reserveProduct', {
      attributes: {
        'order.product_id': productId,
        'order.quantity': quantity,
      },
    });

    try {
      this.loggerService.log(
        `Trying to reserve product ID=${productId}, quantity=${quantity}`,
      );
      span.addEvent('Start reserving product', {
        timestamp: Date.now(),
        quantity,
      });

      const product = this.products.find((p) => p.id === productId);
      if (!product) {
        this.loggerService.log(`Product with ID ${productId} not found`);
        span.setAttribute('error', true);
        span.setStatus({
          code: SpanStatusCode.ERROR,
          message: 'Product not found',
        });
        throw new Error(`Product with ID ${productId} not found`);
      }

      const reservedItem: OrderItem = {
        productId: product.id,
        productName: product.name,
        productPrice: product.price,
        quantity,
      };

      span.setAttribute('product.name', product.name);
      span.setAttribute('product.price', product.price);
      span.addEvent('Product reserved', {
        productName: product.name,
        quantity,
      });

      this.loggerService.log(`Reserved ${quantity} x ${product.name}`);
      return reservedItem;
    } catch (err) {
      span.recordException(err as Error);
      throw err;
    } finally {
      span.end();
    }
  }
}
