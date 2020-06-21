using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFighter {
    class Program {

        static void Main(string[] args) {

            try {

                FightController Controller = new FightController();

                Controller.StartFight();

            }catch(Exception E) {
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
