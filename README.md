Веб сервис, позволяющий хранить и возвращать списки рекламных площадок для заданной локации в запросе. Проект реализован на .Net 8.0

Подготовка к запуску:
1. Клонируйте репозиторий (git clone https://github.com/e6enholz/AdPlatform)
2. Перейдите в папку проекта (cd .../AdPlatform/AdPlatform)
3. Запустите проект (dotnet run)

После запуска UI Swagger доступен по адресу: http://localhost:<порт>/Swagger

Формат тестовых данных:
1. Файл .txt
2. Формат содержимого:<br>
<Название>:путь1,путь2<br>
<Название>:путь1,путь2

Пример тестовых данных:<br>
Яндекс.Директ:/ru<br>
Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik<br>
Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl<br>
Крутая реклама:/ru/svrd

Примеры запросов Git Bash:
1. Загрузка файла
curl -F "file=@/c/путь файла/TEST.txt" http://localhost:<порт>/Platform/upload
2. Поиск платформ
curl "http://localhost:<порт>/Platform/search?location=/ru/svrd/revda"
