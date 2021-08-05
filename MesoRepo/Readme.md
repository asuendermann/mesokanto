Die MesoRepo Solution enthält meine aktuelle Lieblingslösung für den gekapselten Zugriff auf eine DbContext-Implementierung.

Alle wichtigigen Zugriffe auf die Datanbank erfolgen über generische Methoden, die als ExtensionMethods für die DbContet-Klasse definiert sind.

Die gekapselten Methoden geben immer ein DbResult pro betroffener Entity zurück, das detaillierte Information über das Ergebnis der angeforderten Operation enthält.

Innerhalb der Methoden werden die übergebenen Entities vor der Ausführung der angeforderten Operation auf alle möglichen Fehlerfälle geprüft. Die Aktion wird erst ausgeführt, wenn dieÜberprüfung vermuten lässt, dass die Operation ausgeführt werden kann.
