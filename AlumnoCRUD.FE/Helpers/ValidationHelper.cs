using System;
using System.Windows.Forms;

namespace AlumnoCRUD.FE.Helpers
{
    public static class ValidationHelper
    {
        public static bool AreFieldsNotEmpty(params TextBox[] controls)
        {
            foreach (var ctrl in controls)
            {
                if (string.IsNullOrWhiteSpace(ctrl.Text))
                {
                    MessageBox.Show($"El campo '{ctrl.Name.Replace("txt", "")}' es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ctrl.Focus();
                    return false;
                }
            }
            return true;
        }

        public static bool IsOlderThan(DateTime birthDate, int years)
        {
            var age = DateTime.Today.Year - birthDate.Year;
            if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < years)
            {
                MessageBox.Show($"Debe ser mayor de {years} años.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        public static bool IsValidNumber(string text, out int result, string fieldName)
        {
            if (!int.TryParse(text, out result))
            {
                MessageBox.Show($"El campo '{fieldName}' debe ser un número entero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
