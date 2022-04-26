using System;

namespace ConsoleFighter {
    class Program {

        static void Main(string[] args) {

            try {

                FightController Controller = new FightController();

                Controller.StartFight();

            }catch(Exception E) {
                Console.ResetColor();
                Console.Clear();
                Console.WriteLine("- Erro durante a execução...\n\n" +
                    "Mensagem: {0}\n\n" +
                    "- StackTrace \n\n{1}\n\n" +
                    "- Help Microsof: {2}\n", E.Message, E.StackTrace, E.HelpLink);
                Console.ReadKey();
            }
        }
    }
}
