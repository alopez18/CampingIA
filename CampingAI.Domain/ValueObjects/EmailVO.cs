namespace CampingAI.Domain.ValueObjects;
public class EmailVO : Abstractions.ValueObjects.SimpleStringRequiredValueObject<EmailVO> {
    public EmailVO(string value) : base(value) {

        if (!IsValidEmail(value))
            throw new Exceptions.DomainException("Invalid email format");

    }


    private static bool IsValidEmail(string email) {

        try {
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email)
                return false;
        } catch {
            return false;
        }


        var emailRegex = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
                                                                    System.Text.RegularExpressions.RegexOptions.Compiled |
                                                                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return emailRegex.IsMatch(email);
    }
}
