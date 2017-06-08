namespace ConsoleTestApp.Features.B
{
    using System;
    using Miruken.Mvc.Console;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class BView : View<BController>
    {
        private readonly Menu menu;

        public BView()
        {
            var output = new Buffer();
            Content = output;
            output.Header("B View");
            output.WriteLine("Feature B");
            output.Block("At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.");
            menu = new Menu(
                new MenuItem("Back",    ConsoleKey.B, () => Controller.Back()),
                new MenuItem("Forward", ConsoleKey.F, () => Controller.GoToCView()));
            output.WriteLine(menu.ToString());
        }

        public override void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            menu.Listen(keyInfo);
        }
    }
}