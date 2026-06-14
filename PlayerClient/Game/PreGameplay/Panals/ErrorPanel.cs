using MLEM.Ui;
using MLEM.Ui.Elements;
using System.Numerics;

namespace PlayerClient.Game.PreGameplay.Panals
{
    public class ErrorPanel : Panel
    {
        public ErrorPanel(string title, string message, string[] options, Action[] optionActions)
            : base(Anchor.Center, new Vector2(300, 400), Vector2.Zero)
        {
            // Title
            Paragraph titleLabel = new Paragraph(Anchor.AutoCenter, 280, $"<b>{title}</b>");
            AddChild(titleLabel);

            // Divider-ish spacing
            Panel divider = new Panel(Anchor.AutoLeft, new Vector2(1f, 2), Vector2.Zero);
            AddChild(divider);

            // Message (scrollable in case it's long)
            Panel messagePanel = new Panel(Anchor.AutoLeft, new Vector2(1f, 200), Vector2.Zero, scrollOverflow: true);
            Paragraph messageLabel = new Paragraph(Anchor.AutoLeft, 260, message);
            messagePanel.AddChild(messageLabel);
            AddChild(messagePanel);

            // Buttons
            for (int i = 0; i < options.Length; i++)
            {
                Action captured = optionActions[i];
                Button btn = new Button(Anchor.AutoCenter, new Vector2(1f, 40), options[i]);
                btn.OnPressed += _ => captured?.Invoke();
                AddChild(btn);
            }
        }
    }
}
