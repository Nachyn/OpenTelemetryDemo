import { Controller, Post, Query } from '@nestjs/common';
import { WarehouseService } from './warehouse.service';

@Controller('/api/warehouse')
export class WarehouseController {
  constructor(private readonly service: WarehouseService) {}

  @Post()
  getHello(
    @Query('productId') productId: number,
    @Query('quantity') quantity: number,
  ) {
    return this.service.reserveProduct(productId, quantity);
  }
}
