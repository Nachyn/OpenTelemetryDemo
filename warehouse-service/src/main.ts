import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { startOtelSDK } from './tracing';
import { ValidationPipe } from '@nestjs/common';
import { DocumentBuilder, SwaggerModule } from '@nestjs/swagger';

async function bootstrap() {
  startOtelSDK();

  const app = await NestFactory.create(AppModule);

  // Включаем глобальную валидацию
  // npm install class-validator class-transformer
  app.useGlobalPipes(
    new ValidationPipe({
      whitelist: false, // Удаляет поля, которые не описаны в DTO
      forbidNonWhitelisted: true, // Выбрасывает ошибку, если есть лишние поля
      transform: true, // Преобразует входящие данные в типы, указанные в DTO
      transformOptions: {
        enableImplicitConversion: true, // "0" -> 0
      },
    }),
  );

  // SWAGGER UI: http://localhost:3001/api
  SwaggerModule.setup('api', app, () => {
    const config = new DocumentBuilder()
      .setTitle('NRI plugin store')
      .setDescription('NRI plugin API description')
      .setVersion('1.0')
      .build();
    return SwaggerModule.createDocument(app, config);
  });

  app.enableCors();

  await app.listen(process.env.PORT ?? 3001);
}
bootstrap();
