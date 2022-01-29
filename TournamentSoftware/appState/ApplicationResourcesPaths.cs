using System.Collections.Generic;

namespace TournamentSoftware
{
    public class ApplicationResourcesPaths
    {
        public const string appStateJsonPath = "app.json";
        public const string dataBasePath = "db.db";
        public const string registrationBackupPath = "registrationBackup.json";
        public const string judgesBackupPath = "judgesBackup.json";

        public static readonly List<string> allResourcesFilePaths = new List<string> {
            appStateJsonPath,
            dataBasePath,
            registrationBackupPath,
            judgesBackupPath
        };
    }

    public class ApplicationStringValues
    {
        public const string name = "Имя";
        public const string surname = "Фамилия";
        public const string patronymic = "Отчество";
        public const string pseudonym = "Псевдоним";
        public const string leader = "Посевной";
        public const string sex = "Пол";
        public const string height = "Рост";
        public const string weight = "Вес";
        public const string commonRating = "Рейтинг (общий)";
        public const string clubRating = "Рейтинг (клубный)";
        public const string club = "Клуб";
        public const string city = "Город";
        public const string category = "Категория";
        public const string dateOfBirth = "Год рождения";

        public static readonly List<string> stringsConstantsValues = new List<string> {
            name,
            surname,
            patronymic,
            pseudonym,
            leader,
            sex,
            height,
            weight,
            commonRating,
            clubRating,
            club,
            city,
            category,
            dateOfBirth };
    }
}
