import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { WarehouseModule } from './warehouse';
import { LoggerModule } from './logger';

@Module({
  imports: [WarehouseModule, LoggerModule],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
