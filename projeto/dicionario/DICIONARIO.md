# Dicionário de Dados — Sistema de Irrigação

## Tabela: usuario

| Campo | Tipo | Tamanho | Restrições |
|---|---|---|---|
| id_usuario | INT | — | PRIMARY KEY, AUTO_INCREMENT |
| nome | VARCHAR | 100 | — |
| email | VARCHAR | 100 | UNIQUE |
| senha | VARCHAR | 255 | — |
| tipo_usuario | VARCHAR | 20 | — |

---

## Tabela: estufa

| Campo | Tipo | Tamanho | Restrições |
|---|---|---|---|
| id_estufa | INT | — | PRIMARY KEY, AUTO_INCREMENT |
| nome | VARCHAR | 100 | — |
| localizacao | VARCHAR | 150 | — |
| data_instalacao | DATE | — | — |
| status | VARCHAR | 20 | — |
| id_usuario | INT | — | FOREIGN KEY |

### Relacionamento
- id_usuario → usuario(id_usuario)

---

## Tabela: sensor

| Campo | Tipo | Tamanho | Restrições |
|---|---|---|---|
| id_sensor | INT | — | PRIMARY KEY, AUTO_INCREMENT |
| tipo_sensor | VARCHAR | 50 | — |
| data_instalacao | DATE | — | — |
| status | VARCHAR | 20 | — |
| id_estufa | INT | — | FOREIGN KEY |

### Relacionamento
- id_estufa → estufa(id_estufa)

---

## Tabela: leitura_sensor

| Campo | Tipo | Tamanho | Restrições |
|---|---|---|---|
| id_leitura | INT | — | PRIMARY KEY, AUTO_INCREMENT |
| valor | DECIMAL | 5,2 | — |
| data_hora | DATETIME | — | — |
| id_sensor | INT | — | FOREIGN KEY |

### Relacionamento
- id_sensor → sensor(id_sensor)

---

## Tabela: dados_meteorologicos

| Campo | Tipo | Tamanho | Restrições |
|---|---|---|---|
| id_dado | INT | — | PRIMARY KEY, AUTO_INCREMENT |
| temperatura | DECIMAL | 5,2 | — |
| umidade_ar | DECIMAL | 5,2 | — |
| probabilidade_chuva | DECIMAL | 5,2 | — |
| data_hora | DATETIME | — | — |
| id_estufa | INT | — | FOREIGN KEY |

### Relacionamento
- id_estufa → estufa(id_estufa)

---

## Tabela: reservatorio

| Campo | Tipo | Tamanho | Restrições |
|---|---|---|---|
| id_reservatorio | INT | — | PRIMARY KEY, AUTO_INCREMENT |
| capacidade | DECIMAL | 10,2 | — |
| nivel_atual | DECIMAL | 10,2 | — |
| status | VARCHAR | 20 | — |
| id_estufa | INT | — | FOREIGN KEY |

### Relacionamento
- id_estufa → estufa(id_estufa)

---

## Tabela: sistema_irrigacao

| Campo | Tipo | Tamanho | Restrições |
|---|---|---|---|
| id_sistema | INT | — | PRIMARY KEY, AUTO_INCREMENT |
| status | VARCHAR | 20 | — |
| id_estufa | INT | — | FOREIGN KEY |

### Relacionamento
- id_estufa → estufa(id_estufa)

---

## Tabela: irrigacao

| Campo | Tipo | Tamanho | Restrições |
|---|---|---|---|
| id_irrigacao | INT | — | PRIMARY KEY, AUTO_INCREMENT |
| data_inicio | DATETIME | — | — |
| duracao | INT | — | — |
| id_sistema | INT | — | FOREIGN KEY |

### Relacionamento
- id_sistema → sistema_irrigacao(id_sistema)