using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFighter {

    public class Writter : IWritter {

        private static IWritter WritterObj = null;

        private readonly object SyncConsole = new object();

        public int HeightRing { get; private set; }
        public int WidthRing { get; private set; }

        private int LastPositionRight;
        private int LastPositionLeft;

        private Writter(int Width, int Height) {
            HeightRing = Height;
            WidthRing = Width;

            Console.WindowWidth = WidthRing;
            Console.WindowHeight = HeightRing;
            Console.BufferHeight = HeightRing;
            Console.BufferWidth = WidthRing;

            Console.WindowTop = 0;
            Console.WindowLeft = 0;

            Console.OutputEncoding = Encoding.Unicode;
            Console.Title = "Console Fighter";
            //Console.CursorVisible = false;
        }

        public static IWritter GetInstance(int Width = 120, int Height = 30) {
            if (WritterObj == null)
                WritterObj = new Writter(Width, Height);

            return WritterObj;
        }

        public void AddAttackPoint() {
            //throw new NotImplementedException();
        }

        public void AddDefensePoint() {
            //throw new NotImplementedException();
        }

        public void Attack() {
            //throw new NotImplementedException();
        }

        public void Defend() {
           // throw new NotImplementedException();
        }

        public void StayNormal(ref int Pos, PlayerSide Side) {
            /*Atacando: - //
             *Defendendo =╣
             *Normal: ┤│ 
             */

            //Conserta o valor de Pos caso seja inserido algum valor que representaria as extremidades
            if (Pos < 2)
                Pos = 2;
            else if (Pos >= WidthRing - 4)
                Pos = WidthRing - 5;

            //Define as variáveis de cada linha para escrita do personagem
            string firtRow, secondRow, thridRow;

            //Varialvel temporáraia que irá armazenar o valor da última posição
            int LastPos;

            lock (SyncConsole) {

                if (Side == PlayerSide.Left) {

                    //Define o valor temporário da variável LastPos (necessário para não haver duplicação de código)
                    LastPos = LastPositionLeft;

                    //Altera a cor da frente do console
                    Console.ForegroundColor = ConsoleColor.Green;

                    //Define os caracteres que serão escritos em cada linha 
                    firtRow = " ☻│";
                    secondRow = "/█┤";
                    thridRow = "/ \\";

                    //Atualiza o valor da última posição do player esquerdo
                    LastPositionLeft = Pos;
                } else {
                    //Define o valor temporário da variável LastPos (necessário para não haver duplicação de código)
                    LastPos = LastPositionRight;

                    //Altera a cor da frente do console
                    Console.ForegroundColor = ConsoleColor.Blue;

                    //Define os caracteres que serão escritos em cada linha 
                    firtRow = "│☻ ";
                    secondRow = "┤█\\";
                    thridRow = "/ \\";

                    //Atualiza o valor da última posição do player direito
                    LastPositionRight = Pos;
                }

                //Escreve o personagem na posição passada como parâmetro
                WriteCharacter(Pos, LastPos, firtRow, secondRow, thridRow);

                //Reseta as cores do Console para o padrão
                Console.ResetColor();
            }
        }

        private void WriteCharacter(int Pos, int LastPos, string firtRow, string secondRow, string thridRow) {

            //Apaga o personagem da última posição em que se encontrava
            for (int i = 5; i >= 3; i--) {
                Console.SetCursorPosition(LastPos, HeightRing - i);
                Console.Write("   ");
            }

            //Escreve o personagem na posição passada como parâmetro
            Console.SetCursorPosition(Pos, HeightRing - 5);
            Console.Write(firtRow);

            Console.SetCursorPosition(Pos, HeightRing - 4);
            Console.Write(secondRow);

            Console.SetCursorPosition(Pos, HeightRing - 3);
            Console.Write(thridRow);
        }

        public void SubtractAttackPoint() {
            //throw new NotImplementedException();
        }

        public void SubtractDefensePoint() {
            //throw new NotImplementedException();
        }

        public void SubtractLife() {
            //throw new NotImplementedException();
        }

        public void WriteRing() {

            lock (SyncConsole) {

                for (int i = 0; i < HeightRing - 2; i++) {

                    Console.SetCursorPosition(0, i);
                    Console.Write("►►");

                    Console.SetCursorPosition(WidthRing - 2, i);
                    Console.Write("◄◄");

                }

                for (int i = 0; i < WidthRing; i++) {

                    if ((i % 10 == 0 || i == WidthRing - 2 || i == 1) && i != 0) {

                        Console.SetCursorPosition(i, HeightRing - 2);
                        Console.Write('╦');

                        Console.SetCursorPosition(i, HeightRing - 1);
                        Console.Write('║');
                    } else {
                        Console.SetCursorPosition(i, HeightRing - 2);
                        Console.Write('═');
                    }
                }
            }
        }

        public void DeadCharacter() {
            //throw new NotImplementedException();
        }
    }
}
