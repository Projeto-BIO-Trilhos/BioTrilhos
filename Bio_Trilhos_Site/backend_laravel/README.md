# Backend Laravel - Bio Trilhos (Guia de instalação rápida)

Este diretório contém instruções e scripts que ajudam a criar um projeto Laravel local que expõe a API para o frontend do Bio Trilhos.

Objetivo:
- Criar um projeto Laravel funcional conectado ao banco PostgreSQL `biotrilhos`.
- Expor endpoints REST em `/api/*` compatíveis com o frontend (`script.js`).

Observação: este repositório não contém o Laravel completo (pelo peso do `vendor/`). O script `start_laravel.ps1` abaixo cria o projeto usando `composer` no seu computador.

Pré-requisitos (no Windows):
- PHP 8.1+
- Composer
- PostgreSQL 13+
- Git (opcional)

Passos automáticos (executar no PowerShell):

```powershell
# Abra PowerShell como administrador
cd C:\Users\49089220801\Downloads\Bio_Trilhos_Site\backend_laravel
.\start_laravel.ps1
```

O `start_laravel.ps1` fará:
1. Verificar `php` e `composer` instalados
2. Criar projeto Laravel em `backend_laravel_app` usando `composer create-project`
3. Copiar stubs de rotas/controllers (fornecidos) para o projeto
4. Ajustar `.env` para usar PostgreSQL (você confirmará a senha)
5. Executar `composer install` e `php artisan migrate --seed`
6. Iniciar servidor Laravel em `http://localhost:5000`

Após a execução, atualize o frontend `script.js` se necessário para apontar `API_BASE_URL` para `http://localhost:5000/api`.

Se preferir criar manualmente o projeto Laravel, os comandos são:

```powershell
composer create-project --prefer-dist laravel/laravel backend_laravel_app
cd backend_laravel_app
# editar .env com a ConnectionString do PostgreSQL
composer require doctrine/dbal
php artisan migrate
php artisan db:seed
php artisan serve --port=5000
```

Arquivos fornecidos neste diretório (stubs):
- `routes_api_stub.php` - rotas de exemplo para copiar para `routes/api.php`
- `Controllers/` - stubs de controllers para copiar para `app/Http/Controllers/`
- `start_laravel.ps1` - script de automação

Se quiser, executo agora a criação dos stubs adicionais ou ajusto o `script.js` para o host/porta que preferir.