DELETE FROM Pages;
DELETE FROM Workbooks;
DELETE FROM Recordings;

INSERT INTO Workbooks(id, title, subtitle, totalPages, published) VALUES
(1, 'Deutschkurs für Asylbewerber', 'Thannhauser Modell', 2, 1),
(2, 'Willkommen! Die deutsche Sprache - erste Schritte', 'Flüchtlingshilfe München e.V.', 2, 1);

INSERT INTO Recordings (id, length, localPath) VALUES
(1, 127000, 'audios/diktat_seite_01.mp3'),
(2, 462000, 'audios/diktat_seite_02.mp3'),
(3, 127000, 'audios/diktat_seite_03.mp3'),
(4, 368000, 'audios/diktat_seite_04.mp3');

INSERT INTO Pages(workbookID, number, description, teacherTrackId) VALUES
(1, 1, 'Begrüßung, Vorstellung und Familie', 1),
(1, 2, 'Wie geht es Ihnen?', 2),
(2, 1, 'Das Alphabet', 3),
(2, 2, 'Erste Gespräche', 4);
