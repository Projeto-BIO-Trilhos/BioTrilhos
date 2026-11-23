FROM php:8.3-cli

RUN apt-get update && apt-get install -y \
    git unzip libpq-dev libicu-dev libonig-dev libzip-dev \
    && docker-php-ext-install pdo_pgsql pgsql mbstring intl zip \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

WORKDIR /var/www/html
COPY . .

RUN composer install --no-dev --optimize-autoloader --no-interaction
RUN php artisan config:clear || true

EXPOSE 10000
CMD ["sh", "-c", "php artisan migrate --force && php artisan config:clear && php artisan serve --host=0.0.0.0 --port=${PORT:-10000}"]