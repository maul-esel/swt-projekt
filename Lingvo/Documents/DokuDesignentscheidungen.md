# Dokumentation Designentscheidungen
Version 1.0
12.01.17
## Seiten verwalten: Proxy Pattern
Für das Speichern, Herunterladen und sonstige Verwaltung von Arbeitsheft-Seiten wird das Proxy-Pattern verwendet. Grundlegender Vorteil hiervon ist, dass auf App-Seite nicht immer vollständige Seiten inklusive Audio-Dateien geladen werden müssen und so der Traffic minimiert wird.
Für die Realisiserung des Patterns gibt es 3 Klassen: das Interface IPage und die Implementierungsklassen Page und PageProxy. Workbook Objekte speichern eine Liste von IPage Objekten, wobei auf Backend-Seite Page Objekte und auf App-Seite PageProxy Objekte in dieser Liste zu finden sind. Wird auf App-Seite eine Seite heruntergeladen, muss beim Proxy-Objekt Resolve() aufgerufen werden. Dabei wird das "echte" Seitenobjekt geladen und dem Proxy-Objekt angehängt. Im Workbook Objekt bleibt aber weiterhin das Proxy gespeichert. Um herauszufinden, welche Seiten wirklich heruntergeladen sind, muss man also die Referenz der Proxies auf ihr Original überprüfen.
Auf dem Server existieren natürlich keine Proxies. Deshalb ist im Designklassendiagramm die PageProxy Klasse im Client Paket zu finden, während IPage und Page sowohl von Backend als auch von der App benutzt werden und sind deshalb im Common Paket.

## Controller
Zugriffe der Benutzer auf jegliche Daten, egal ob im Server oder in der App, werden über Controller geregelt.

### Backend
Für das Redaktionssystem existieren vier Controller. Der erste ist der AccountController, der zuständig ist für die Verwaltung, Login und Logout der Redakteure. WorkbookController und PageController sind zuständig für alle Operationen, die ein Redakteur über die UI des Backends tätigen kann. Der WorkbookController kapselt dabei alle Operationen, die mit Workbooks zu tun haben (erstellen, aktualisieren, löschen, veröffentlichen). Ebenso ist der PageController für die Operationen auf den Pages verantwortlich (erstellen, löschen, aktualisieren). Der vierte Controller ist der AppController. Er bildet die Schnittstelle zur App, stellt also die Operationen zur Verfügung, die in der App für den Download benötigt werden. Alle vier Controller kapseln stark zusammenhängende Operationen, weshalb die Entscheidung auf diese Aufteilung fiel.

### Frontend
Bei der App fiel die Entscheidung auf zwei Use Case Controller. Das lag an der großen Anzahl an Methoden, die in der App benötigt werden und zwischen denen teilweise kein Zusammenhang besteht, weshalb sie auch nicht in einer Klasse kombiniert werden sollten. Somit wurden alle Methoden, die mit Use Cases zu lokal eingesprochenen Lehrerspuren (also Teachermemos im Designklassendiagramm) zu tun haben, in den TeacherMemoController gepackt und alle Operationen, die sich auf Use Cases beziehen, die der Bearbeitung von Seiten aus Arbeitsheften zu tun haben, in den StudentPageController gelagert. 
Durch die Wahl von Use Case Controllern kann außerdem eine flüssige Nutzerführung erreicht werden. Den Flüchtlingen soll die Benutzung der App möglichst leicht und intuitiv gemacht werden. Außerdem somit einfacher erreicht werden, dass Operationen nur in einer zulässigen Kombination im Rahmen der beschrieben Use Cases ausgeführt werden.

## Verwendung von Audiofunktionalitäten
Wie im Designklassendiagramm ersichtlich, werden für die Anbindung von Audiobibliotheken Interfaces benutzt. Dies liegt daran, dass Audiofunktionalität sehr plattformspezifisch ist und auf Android bzw. iOS unterschiedlich implementiert werden muss. Damit aber trotzdem einheitlich auf Player bzw. Recorder zugegriffen werden kann, existieren die Interfaces IPlayer und IRecorder, die beide jeweils von einer android-spezifischen und einer iOS-spezifischen Klasse implementiert werden.
