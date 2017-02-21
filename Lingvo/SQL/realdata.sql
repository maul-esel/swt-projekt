-- phpMyAdmin SQL Dump
-- version 4.5.1
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1:54133
-- Generation Time: Feb 15, 2017 at 02:10 PM
-- Server version: 5.7.9
-- PHP Version: 5.5.38

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `localdb`
--

-- --------------------------------------------------------

--
-- Table structure for table `editors`
--

--
-- Truncate table before insert `editors`
--

TRUNCATE TABLE `editors`;
--
-- Dumping data for table `editors`
--

INSERT INTO `editors` (`name`, `passwordHash`) VALUES
('DEV', 'AQAAAAEAACcQAAAAEObagOrjXY6h0oymHQWd9na59TRLBdb9etP31ZWMN5OfbCopKZazN4HH59t1SELvPA=='),
('GIETMANN', 'AQAAAAEAACcQAAAAEM75qbd85ybQJzWImNGHOoJ7gEu59ANQ2aFf1E86+uCA9M4deXUDixFOTHMfFjUSQQ==');

--
-- Table structure for table `recordings`
--

--
-- Truncate table before insert `recordings`
--

DELETE FROM `recordings`;
--
-- Dumping data for table `recordings`
--

INSERT INTO `recordings` (`id`, `creationTime`, `duration`, `localPath`) VALUES
(5, '2017-02-15 18:27:10', 94590, 'uploaded_e1391f3e-f243-48de-943a-4695a753d423'),
(6, '2017-02-15 18:30:26', 127295, 'uploaded_15f845b0-617c-4c51-8878-35ace0d2978e'),
(7, '2017-02-15 18:31:07', 118727, 'uploaded_5281cbf6-21e3-4804-8340-8a9c990f2d70'),
(8, '2017-02-15 18:31:51', 115592, 'uploaded_705e4f6f-8f98-4310-95b9-df08ae334d8d'),
(9, '2017-02-15 18:32:54', 103889, 'uploaded_b9117615-732f-45c7-bb3d-24e0c0f68f22'),
(10, '2017-02-15 18:33:39', 67475, 'uploaded_8de801f1-d8aa-4f04-a2f2-6df9952c26a0'),
(11, '2017-02-15 18:34:21', 199733, 'uploaded_26c66ad7-b03d-4b7e-b246-15d02ab59db0'),
(12, '2017-02-15 18:34:57', 300957, 'uploaded_af7f90e0-88a7-4ac1-b81a-b4d3c21e6291'),
(13, '2017-02-15 18:36:33', 85499, 'uploaded_ff8600c9-b60d-48bb-9818-cc65a8206486'),
(14, '2017-02-15 18:37:07', 110368, 'uploaded_ad05cf07-15bb-4513-abf2-61e869632beb'),
(15, '2017-02-15 18:38:29', 139729, 'uploaded_c584b011-b79b-4064-8fe0-1e31c4404c77'),
(16, '2017-02-15 18:39:07', 131553, 'uploaded_cd4668f8-28a0-4778-9993-c888843421d3'),
(17, '2017-02-15 18:39:57', 119667, 'uploaded_17d6267a-17d5-4b38-a34b-c8b3aef8673a'),
(18, '2017-02-15 18:40:48', 115958, 'uploaded_80b0116c-df38-41d2-8aa4-e54a3227bafd'),
(19, '2017-02-15 18:42:24', 61885, 'uploaded_c0d345eb-3303-404d-badf-be06cca1aad1'),
(20, '2017-02-15 18:43:02', 121705, 'uploaded_133fef22-7f27-47c1-93b8-6cb475cd8ad4'),
(21, '2017-02-15 18:43:51', 137117, 'uploaded_944c9e44-e2bf-4418-a985-80c3987f7f0e'),
(22, '2017-02-15 18:44:28', 101277, 'uploaded_9c31ece2-1caf-45fe-8cb4-7f19bec3cffc'),
(23, '2017-02-15 18:44:59', 110002, 'uploaded_6caef693-7e0d-485a-94e7-2fa27758772e'),
(24, '2017-02-15 18:46:22', 117813, 'uploaded_2b00b952-286a-4a09-b701-24b32fb2fc5b'),
(25, '2017-02-15 18:47:10', 85290, 'uploaded_d0689a45-6c8b-4710-a383-970a18e28a08'),
(26, '2017-02-15 18:51:47', 104804, 'uploaded_d9dfdc45-8bca-4b46-a0ee-2004ba5b1e10'),
(27, '2017-02-15 18:53:07', 159792, 'uploaded_fe5ccc6a-90c1-49bc-9e31-2b0bac4951a7'),
(28, '2017-02-15 19:37:01', 96262, 'uploaded_55342dc6-dd89-4687-8b1a-3071dd866aa5'),
(29, '2017-02-15 19:37:47', 154045, 'uploaded_50c60c26-4a17-40cf-bf91-f7b5df91bae3'),
(30, '2017-02-15 19:38:20', 126903, 'uploaded_faeec2fc-8dd2-4abe-b9c8-d46e38f23644'),
(31, '2017-02-15 19:39:07', 162769, 'uploaded_99908a9b-54e8-4c83-8d78-86ac9b78cb36'),
(32, '2017-02-15 19:39:43', 170554, 'uploaded_89322fd4-3284-43c5-9dbd-2c937d5041ec'),
(33, '2017-02-15 19:40:31', 116324, 'uploaded_8afa0c95-ed7e-476f-88ed-e3cd4ab8ca96'),
(34, '2017-02-15 19:41:06', 129881, 'uploaded_a28d0d49-c1e3-4419-9df5-9301613d1d98'),
(35, '2017-02-15 19:41:37', 75076, 'uploaded_58468cd0-4d70-49ab-8d52-37845fd15f24'),
(36, '2017-02-15 19:42:10', 92369, 'uploaded_a0e24efc-18c0-4235-a9f7-bffedbea2570'),
(37, '2017-02-15 19:43:10', 72490, 'uploaded_05d8b525-f394-479a-a7e3-ec12885292bd'),
(38, '2017-02-15 19:43:52', 52977, 'uploaded_4612b9a1-34fc-4d69-92ed-d384667a58f2'),
(39, '2017-02-15 19:44:45', 126720, 'uploaded_c924e6fd-1dcc-4702-90f9-526e5e512af2'),
(40, '2017-02-15 19:45:28', 47778, 'uploaded_ce585a8b-04f8-485a-84e2-49c0ad62d32e'),
(41, '2017-02-15 19:46:12', 74345, 'uploaded_f77ed84f-f2f6-4f13-8b09-0fdd1cd7cdae'),
(42, '2017-02-15 19:46:53', 120033, 'uploaded_ce66136e-8dcf-4a50-9de0-23f44d07dc46'),
(43, '2017-02-15 19:50:57', 126538, 'uploaded_0810d5a8-cde2-4a08-a27b-f2ffd4a1d379'),
(44, '2017-02-15 19:51:42', 461845, 'uploaded_6006ebf3-fc44-4074-9d63-bf11535dc6d3'),
(45, '2017-02-15 19:54:59', 126903, 'uploaded_962fef52-0bb2-4174-90d9-325785dee391'),
(46, '2017-02-15 20:02:43', 368040, 'uploaded_eb07564f-0300-4c8a-a7f4-5f71c49d62ac'),
(47, '2017-02-15 20:03:25', 489692, 'uploaded_dd3dea46-f5eb-42a1-ab68-36f6c6a2d6ab'),
(48, '2017-02-15 20:04:01', 316944, 'uploaded_22f7a674-f45f-4b2e-a163-733d937d7d3b'),
(49, '2017-02-15 20:04:33', 310074, 'uploaded_b944370c-9173-4ce5-bd5e-b488f5dde0bd'),
(50, '2017-02-15 20:09:56', 449202, 'uploaded_5beb7f39-5047-430e-8379-1580a79eb0aa'),
(51, '2017-02-15 20:10:46', 569392, 'uploaded_d5d50537-ba12-4d71-9c28-770628d1879d'),
(52, '2017-02-15 21:25:37', 258978, 'uploaded_5e4f8697-70b2-45b0-8fb6-d5730eb66c7d'),
(53, '2017-02-15 21:26:22', 111125, 'uploaded_19a38c07-6d25-42d7-a487-fabddeaa9047'),
(54, '2017-02-15 21:26:51', 110002, 'uploaded_f527db59-fa7c-456a-a0d2-9eb7ded29bd8'),
(55, '2017-02-15 21:27:34', 81398, 'uploaded_08e7dafc-cfc7-43f7-9559-d7abe80813d2'),
(56, '2017-02-15 21:28:09', 263811, 'uploaded_837d667b-992f-4830-a9c1-29baebc3c817'),
(57, '2017-02-15 21:30:37', 176876, 'uploaded_11930aac-1a2f-4f09-bc8c-41aa07e68bfc'),
(58, '2017-02-15 21:31:36', 192105, 'uploaded_c9f9830f-aa45-4dde-b3bc-f5c7d9875373'),
(59, '2017-02-15 21:32:52', 102400, 'uploaded_a2c1f3e8-8b37-40f6-8bff-ecf61155d296'),
(60, '2017-02-15 21:33:38', 96627, 'uploaded_dd4cd3c5-b2ae-42f0-b3b7-519bc2e34789'),
(61, '2017-02-15 21:45:02', 167027, 'uploaded_5ea09b77-6e94-4196-8700-254177284b92'),
(62, '2017-02-15 21:46:02', 102400, 'uploaded_7db80284-c332-4f72-942c-c6cf9d6cb0d5'),
(63, '2017-02-15 21:46:39', 189519, 'uploaded_56dc0a67-dd0f-45dc-bcb1-40c238eb45a7'),
(64, '2017-02-15 21:48:12', 141767, 'uploaded_d71387b6-e308-47f7-9e93-7616d61d7483'),
(65, '2017-02-15 21:48:47', 171129, 'uploaded_23137f7f-6c2f-4090-959d-7e4e7bc7d846'),
(66, '2017-02-15 21:49:14', 83253, 'uploaded_a039b52b-5363-4703-bb71-26b5ab1e3589'),
(67, '2017-02-15 21:49:50', 86779, 'uploaded_6a500f99-33b0-4bb3-a122-e5b190b99843'),
(68, '2017-02-15 21:50:24', 96262, 'uploaded_b9bb0b1f-4f9d-4b35-b5b0-fdc185670d81'),
(69, '2017-02-15 21:51:25', 77506, 'uploaded_55486239-6e96-4db8-b6cf-c0e8dc01ec4d'),
(70, '2017-02-15 21:51:55', 81398, 'uploaded_e81bdfa3-b5bf-4681-80b5-eec0677b77fe'),
(71, '2017-02-15 21:52:36', 113712, 'uploaded_ec2a0a8f-9985-406f-8cd0-c711c921ca68'),
(72, '2017-02-15 21:53:08', 51462, 'uploaded_bf3485bf-8cea-42a7-8ad8-39777d093b38'),
(73, '2017-02-15 21:53:41', 61153, 'uploaded_792611d7-8cf6-4e2a-963e-e2b0fab29fce'),
(74, '2017-02-15 21:57:14', 48327, 'uploaded_1b0da310-3a7e-4db8-af00-df466d5a726b'),
(75, '2017-02-15 21:57:47', 149212, 'uploaded_d8406ac8-7228-469b-96ce-e7cb8e321526'),
(76, '2017-02-15 22:00:03', 93858, 'uploaded_22b58180-94e5-4690-8bd3-9012c4449d47'),
(77, '2017-02-15 22:01:19', 78603, 'uploaded_b6cf12c0-9955-4017-aca2-d9eef74041a5'),
(78, '2017-02-15 22:01:52', 217574, 'uploaded_69d8233f-e35c-48be-805e-25d827fd1fdf');

-- --------------------------------------------------------

--
-- Table structure for table `workbooks`
--

--
-- Truncate table before insert `workbooks`
--

DELETE FROM `workbooks`;
--
-- Dumping data for table `workbooks`
--

INSERT INTO `workbooks` (`id`, `title`, `subtitle`, `totalPages`, `lastModified`, `isPublished`) VALUES
(3, 'Thannhauser Modell', 'Deutschkurs für Anfänger', 38, '2017-02-15 19:46:53', 1),
(5, 'Diktate und mehr', 'Tauschring Gelderland', 9, '2017-02-15 20:28:53', 1),
(7, 'Flüchtlingshilfe München', NULL, 27, '2017-02-15 22:02:15', 1);



-- --------------------------------------------------------

--
-- Table structure for table `pages`
--

--
-- Truncate table before insert `pages`
--

DELETE FROM `pages`;
--
-- Dumping data for table `pages`
--

INSERT INTO `pages` (`id`, `workbookID`, `number`, `description`, `teacherTrackId`, `studentTrackId`) VALUES
(5, 3, 3, 'Begrüßung, Vorstellung und Familie', 5, NULL),
(6, 3, 4, 'Wie geht es Ihnen?', 6, NULL),
(7, 3, 5, 'Ich mache etwas', 7, NULL),
(8, 3, 6, 'Tagesablauf und Tageszeit', 8, NULL),
(9, 3, 7, 'Was machen Sie heute?', 9, NULL),
(10, 3, 8, 'Was machst du heute?', 10, NULL),
(11, 3, 9, 'Wochentage, Zahlen, Farben und Uhrzeit', 11, NULL),
(12, 3, 10, 'Zahlen', 12, NULL),
(13, 3, 11, 'Wie spät ist es?', 13, NULL),
(14, 3, 12, 'Haushalt und wohnen', 14, NULL),
(15, 3, 13, 'Wo ist...?', 15, NULL),
(16, 3, 14, 'Wie ist es?', 16, NULL),
(17, 3, 15, 'Ich gehe einkaufen', 17, NULL),
(18, 3, 16, 'Einkaufszettel', 18, NULL),
(19, 3, 17, 'Einkaufsprospekt', 19, NULL),
(20, 3, 18, 'Körper und Gesundheit', 20, NULL),
(21, 3, 19, 'Mein Gesicht', 21, NULL),
(22, 3, 20, 'Ich bin krank', 22, NULL),
(23, 3, 21, 'Beim Arzt', 23, NULL),
(24, 3, 22, 'Kleidung', 24, NULL),
(25, 3, 23, 'Im Bekleidungsgeschäft', 25, NULL),
(26, 3, 24, 'Fahduma kauft ein', 26, NULL),
(27, 3, 25, 'Die Jahreszeiten', 27, NULL),
(28, 3, 26, 'Das Wetter in Deutschland', 28, NULL),
(29, 3, 27, 'Berufe', 29, NULL),
(30, 3, 28, 'Wer arbeitet wo?', 30, NULL),
(31, 3, 29, 'Ein Kind geht', 31, NULL),
(32, 3, 30, 'Was hast du gemacht?', 32, NULL),
(33, 3, 31, 'Kommunikation', 33, NULL),
(34, 3, 32, 'Kein Leben ohne Smartphone?', 34, NULL),
(35, 3, 33, 'Verkehr und Orientierung', 35, NULL),
(36, 3, 34, 'Der Zugfahrplan', 36, NULL),
(37, 3, 35, 'Suche im Stadtplan', 37, NULL),
(38, 3, 36, 'Behörden für Asylbewerber', 38, NULL),
(39, 3, 37, 'Religion', 39, NULL),
(40, 3, 38, 'Feste in Deutschland', 40, NULL),
(41, 3, 39, 'Mein Steckbrief', 41, NULL),
(42, 3, 40, 'Das kann ich gut', 42, NULL),
(43, 5, 1, 'Zuhause üben', 43, NULL),
(44, 5, 2, 'Alles beginnt zu blühen', 44, NULL),
(45, 5, 3, 'Die Fahrt zum Bauernhof', 45, NULL),
(46, 5, 4, 'Viel zu tun', 46, NULL),
(47, 5, 5, 'Der Igel', 47, NULL),
(48, 5, 6, 'Der Ausflug', 48, NULL),
(49, 5, 7, 'Wir und Europa', 49, NULL),
(50, 5, 8, 'Ich werde Anstreicherhelfer', 50, NULL),
(51, 5, 9, 'Flüchtlinge', 51, NULL),
(52, 7, 4, 'Willkommen', 52, NULL),
(53, 7, 6, 'Erste Gespräche', 53, NULL),
(54, 7, 7, 'Schreibe bitte', 54, NULL),
(55, 7, 8, 'Begrüßung und Verabschiedung', 55, NULL),
(56, 7, 9, 'Die Zahlen', 56, NULL),
(57, 7, 10, 'Die Uhr', 57, NULL),
(58, 7, 11, 'Wie spät ist es?', 58, NULL),
(59, 7, 12, 'Bus und U-Bahn', 59, NULL),
(60, 7, 13, 'Schreibe bitte', 60, NULL),
(61, 7, 14, 'Essen und Trinken', 61, NULL),
(62, 7, 15, 'Essen und Trinken', 62, NULL),
(63, 7, 16, 'Einkaufen', 63, NULL),
(64, 7, 17, 'Im Supermarkt', 64, NULL),
(65, 7, 18, 'Kleidung', 65, NULL),
(66, 7, 19, 'Kleidung', 66, NULL),
(67, 7, 20, 'Die Famimlie', 67, NULL),
(68, 7, 21, 'Schreibe bitte', 68, NULL),
(69, 7, 22, 'Das Gesicht', 69, NULL),
(70, 7, 23, 'Der Körper', 70, NULL),
(71, 7, 24, 'Gesundheit und Krankheit', 71, NULL),
(72, 7, 25, 'Wichtig', 72, NULL),
(73, 7, 26, 'Die Possesivpronomen', 73, NULL),
(74, 7, 27, 'Spreche und schreibe', 74, NULL),
(75, 7, 28, 'Von Tag zu Tag', 75, NULL),
(76, 7, 29, 'Was ist in meinem Zimmer?', 76, NULL),
(77, 7, 30, 'Was tue ich in meinem Zimmer', 77, NULL),
(78, 7, 31, 'Ich sitze', 78, NULL);

-- --------------------------------------------------------
--
-- Indexes for dumped tables
--

--
-- AUTO_INCREMENT for table `pages`
--
ALTER TABLE `pages`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=79;
--
-- AUTO_INCREMENT for table `recordings`
--
ALTER TABLE `recordings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=79;
--
-- AUTO_INCREMENT for table `workbooks`
--
ALTER TABLE `workbooks`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;
--
-- Constraints for dumped tables
--
