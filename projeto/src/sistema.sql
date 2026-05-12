-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Tempo de geração: 12/05/2026 às 12:05
-- Versão do servidor: 8.4.7
-- Versão do PHP: 8.3.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Banco de dados: `sistema`
--

-- --------------------------------------------------------

--
-- Estrutura para tabela `dados_meteorologicos`
--

DROP TABLE IF EXISTS `dados_meteorologicos`;
CREATE TABLE IF NOT EXISTS `dados_meteorologicos` (
  `id_dado` int NOT NULL AUTO_INCREMENT,
  `temperatura` decimal(5,2) DEFAULT NULL,
  `umidade_ar` decimal(5,2) DEFAULT NULL,
  `probabilidade_chuva` decimal(5,2) DEFAULT NULL,
  `data_hora` datetime DEFAULT NULL,
  `id_estufa` int DEFAULT NULL,
  PRIMARY KEY (`id_dado`),
  KEY `id_estufa` (`id_estufa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `estufa`
--

DROP TABLE IF EXISTS `estufa`;
CREATE TABLE IF NOT EXISTS `estufa` (
  `id_estufa` int NOT NULL AUTO_INCREMENT,
  `nome` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `localizacao` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `data_instalacao` date DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_usuario` int DEFAULT NULL,
  PRIMARY KEY (`id_estufa`),
  KEY `id_usuario` (`id_usuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `irrigacao`
--

DROP TABLE IF EXISTS `irrigacao`;
CREATE TABLE IF NOT EXISTS `irrigacao` (
  `id_irrigacao` int NOT NULL AUTO_INCREMENT,
  `data_inicio` datetime DEFAULT NULL,
  `duracao` int DEFAULT NULL,
  `id_sistema` int DEFAULT NULL,
  PRIMARY KEY (`id_irrigacao`),
  KEY `id_sistema` (`id_sistema`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `leitura_sensor`
--

DROP TABLE IF EXISTS `leitura_sensor`;
CREATE TABLE IF NOT EXISTS `leitura_sensor` (
  `id_leitura` int NOT NULL AUTO_INCREMENT,
  `valor` decimal(5,2) DEFAULT NULL,
  `data_hora` datetime DEFAULT NULL,
  `id_sensor` int DEFAULT NULL,
  PRIMARY KEY (`id_leitura`),
  KEY `id_sensor` (`id_sensor`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `reservatorio`
--

DROP TABLE IF EXISTS `reservatorio`;
CREATE TABLE IF NOT EXISTS `reservatorio` (
  `id_reservatorio` int NOT NULL AUTO_INCREMENT,
  `capacidade` decimal(10,2) DEFAULT NULL,
  `nivel_atual` decimal(10,2) DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_estufa` int DEFAULT NULL,
  PRIMARY KEY (`id_reservatorio`),
  KEY `id_estufa` (`id_estufa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `sensor`
--

DROP TABLE IF EXISTS `sensor`;
CREATE TABLE IF NOT EXISTS `sensor` (
  `id_sensor` int NOT NULL AUTO_INCREMENT,
  `tipo_sensor` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `data_instalacao` date DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_estufa` int DEFAULT NULL,
  PRIMARY KEY (`id_sensor`),
  KEY `id_estufa` (`id_estufa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `sistema_irrigacao`
--

DROP TABLE IF EXISTS `sistema_irrigacao`;
CREATE TABLE IF NOT EXISTS `sistema_irrigacao` (
  `id_sistema` int NOT NULL AUTO_INCREMENT,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_estufa` int DEFAULT NULL,
  PRIMARY KEY (`id_sistema`),
  KEY `id_estufa` (`id_estufa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `usuario`
--

DROP TABLE IF EXISTS `usuario`;
CREATE TABLE IF NOT EXISTS `usuario` (
  `id_usuario` int NOT NULL AUTO_INCREMENT,
  `nome` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `senha` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tipo_usuario` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_usuario`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Restrições para tabelas despejadas
--

--
-- Restrições para tabelas `dados_meteorologicos`
--
ALTER TABLE `dados_meteorologicos`
  ADD CONSTRAINT `dados_meteorologicos_ibfk_1` FOREIGN KEY (`id_estufa`) REFERENCES `estufa` (`id_estufa`);

--
-- Restrições para tabelas `estufa`
--
ALTER TABLE `estufa`
  ADD CONSTRAINT `estufa_ibfk_1` FOREIGN KEY (`id_usuario`) REFERENCES `usuario` (`id_usuario`);

--
-- Restrições para tabelas `irrigacao`
--
ALTER TABLE `irrigacao`
  ADD CONSTRAINT `irrigacao_ibfk_1` FOREIGN KEY (`id_sistema`) REFERENCES `sistema_irrigacao` (`id_sistema`);

--
-- Restrições para tabelas `leitura_sensor`
--
ALTER TABLE `leitura_sensor`
  ADD CONSTRAINT `leitura_sensor_ibfk_1` FOREIGN KEY (`id_sensor`) REFERENCES `sensor` (`id_sensor`);

--
-- Restrições para tabelas `reservatorio`
--
ALTER TABLE `reservatorio`
  ADD CONSTRAINT `reservatorio_ibfk_1` FOREIGN KEY (`id_estufa`) REFERENCES `estufa` (`id_estufa`);

--
-- Restrições para tabelas `sensor`
--
ALTER TABLE `sensor`
  ADD CONSTRAINT `sensor_ibfk_1` FOREIGN KEY (`id_estufa`) REFERENCES `estufa` (`id_estufa`);

--
-- Restrições para tabelas `sistema_irrigacao`
--
ALTER TABLE `sistema_irrigacao`
  ADD CONSTRAINT `sistema_irrigacao_ibfk_1` FOREIGN KEY (`id_estufa`) REFERENCES `estufa` (`id_estufa`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
