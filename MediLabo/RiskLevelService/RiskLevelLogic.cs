using RiskLevelService.Dto;

namespace RiskLevelService
{
    public static class RiskLevelLogic
    {

        public static string CalculateRisk(PatientDto patient, List<NoteDto> notes)
        {
            // Règles de déclencheurs, en minuscules pour comparaison insensible à la casse
            var triggers = new[]
            {
        "hémoglobine a1c",
        "microalbumine",
        "taille",
        "poids",
        "fumeur",
        "fumer",
        "fumeuse",
        "anormal",
        "cholestérol",
        "vertiges",
        "vertige",
        "rechute",
        "réaction",
        "anticorps"
    };

            // Calcul de l'âge exact
            var age = GetAccurateAge(patient.DateOfBirth);

            // Détection des déclencheurs dans toutes les notes, uniques
            var foundTriggers = new HashSet<string>();
            foreach (var note in notes)
            {
                var content = note.Content.ToLowerInvariant();
                foreach (var trigger in triggers)
                {
                    if (content.Contains(trigger))
                    {
                        foundTriggers.Add(trigger);
                    }
                }
            }

            var countTriggers = foundTriggers.Count;
            var isMale = string.Equals(patient.Gender, "m", StringComparison.OrdinalIgnoreCase);

            if (countTriggers == 0)
                return "None";
            if (countTriggers >= 2 && countTriggers <= 5 && age > 30)
                return "Borderline";

            if (age < 30)
            {
                if (isMale)
                {
                    if (countTriggers >= 5) return "Early onset";
                    if (countTriggers >= 3) return "In Danger";
                }
                else
                {
                    if (countTriggers >= 7) return "Early onset";
                    if (countTriggers >= 4) return "In Danger";
                }
            }
            else // age >= 30
            {
                if (countTriggers >= 8) return "Early onset";
                if (countTriggers == 6 || countTriggers == 7) return "In Danger";
            }

            return "None";
        }

        private static int GetAccurateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

}
