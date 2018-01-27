
-- phpMyAdmin SQL Dump
-- version 3.5.2.2
-- http://www.phpmyadmin.net
--
-- Client: localhost
-- Généré le: Mer 26 Octobre 2016 à 09:31
-- Version du serveur: 10.0.20-MariaDB
-- Version de PHP: 5.2.17

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de données: `u363302052_rolex`
--

-- --------------------------------------------------------

--
-- Structure de la table `Scores`
--

CREATE TABLE IF NOT EXISTS `Scores` (
  `Pseudo` tinytext CHARACTER SET utf8mb4 NOT NULL,
  `Level` tinytext NOT NULL,
  `Id_score` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `Temps` tinytext NOT NULL,
  `Nb_essais` tinytext NOT NULL,
  `Date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id_score`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=2 ;

--
-- Contenu de la table `Scores`
--

INSERT INTO `Scores` (`Pseudo`, `Level`, `Id_score`, `Temps`, `Nb_essais`, `Date`) VALUES
('Romain', 'Level 1', 1, '2.024', '0', '2016-10-24 14:29:29');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
