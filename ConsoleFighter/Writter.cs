using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleFighter {

    public static class StringExt {
        public static string PadBoth(this string str, int length, char character = ' ') => str.PadLeft((length - str.Length) / 2 + str.Length, character).PadRight(length, character);
    }

    public class Writter : IWritter {

        private static IWritter WritterObj = null;

        private readonly object SyncConsole = new object();

        public int HeightRing { get; private set; }
        public int WidthRing { get; private set; }

        public ConsoleColor ColorRight { get; set; }
        public ConsoleColor ColorLeft { get; set; }

        private int LastPositionRight;
        private int LastPositionLeft;

        private int LeftPositionRing;
        private int RightPositionRing;

        private string NamePlayerLeft;
        private string NamePlayerRight;

        private Writter(int Width, int Height) {
            HeightRing = Height;
            WidthRing = Width;

            ColorRight = ConsoleColor.Blue;
            ColorLeft = ConsoleColor.Green;

            NamePlayerLeft = "PLAYER";
            NamePlayerRight = "COMPUTADOR";

            RestartValues();

            Console.WindowWidth = WidthRing;
            Console.WindowHeight = HeightRing;
            Console.BufferHeight = HeightRing;
            Console.BufferWidth = WidthRing;

            Console.WindowTop = 0;
            Console.WindowLeft = 0;

            Console.OutputEncoding = Encoding.Unicode;
            Console.Title = "Console Fighter";
            Console.CursorVisible = false;
        }

        public void RestartValues() {
            LastPositionLeft = 2;
            LastPositionRight = WidthRing - 7;

            LeftPositionRing = 1;
            RightPositionRing = WidthRing - 1;
        }

        public static IWritter GetInstance(int Width = 120, int Height = 30) {
            if (WritterObj == null)
                WritterObj = new Writter(Width, Height);

            return WritterObj;
        }


        public void Attack(int Pos, PlayerSide Side) {

            //Define as variáveis de cada linha para escrita do personagem
            string firtRow, secondRow;

            int PosSecond = -1;

            lock (SyncConsole) {

                if (Side == PlayerSide.Left) {

                    Pos += 2;

                    //Altera a cor da frente do console
                    Console.ForegroundColor = ColorLeft;

                    //Define os caracteres que serão escritos em cada linha 
                    firtRow = "  /";
                    secondRow = "-/";

                } else {

                    //Altera a cor da frente do console
                    Console.ForegroundColor = ColorRight;

                    //Define os caracteres que serão escritos em cada linha 
                    firtRow = "\\  ";
                    secondRow = "\\-";

                    Pos -= 2;
                    PosSecond = Pos + 1;
                }

                //Escreve o personagem na posição passada como parâmetro
                WriteCharacter(Pos, firtRow, secondRow, "", PosSecond);

                //Reseta as cores do Console para o padrão
                Console.ResetColor();
            }
        }

        public void Defend(int Pos, PlayerSide Side) {

            //Define as variáveis de cada linha para escrita do personagem
            string firstRow, secondRow;

            int SecondPos;

            lock (SyncConsole) {

                if (Side == PlayerSide.Left) {

                    //Altera a cor da frente do console
                    Console.ForegroundColor = ColorLeft;

                    //Define os caracteres que serão escritos em cada linha 
                    firstRow = " ";
                    secondRow = "  █=╣";

                    Pos += 2;
                    SecondPos = Pos - 3;
                } else {

                    //Altera a cor da frente do console
                    Console.ForegroundColor = ColorRight;

                    //Define os caracteres que serão escritos em cada linha 
                    firstRow = " ";
                    secondRow = "╠=█ ";

                    SecondPos = Pos - 1;

                }

                //Escreve o personagem na posição passada como parâmetro
                WriteCharacter(Pos, firstRow, secondRow, "", SecondPos);

                //Reseta as cores do Console para o padrão
                Console.ResetColor();
            }
        }

        public bool IsAtEdge(int Pos) {

            lock (SyncConsole) {
                if (Pos <= LeftPositionRing + 2)
                    return true;
                else if (Pos >= RightPositionRing - 5)
                    return true;
                else
                    return false;
            }
        }

        public int StayNormal(ref int Pos, PlayerSide Side, ConsoleColor Color = ConsoleColor.Black, bool EraseAtk = false) {
            /*Atacando: - //
             *Defendendo =╣
             *Normal: ┤│ 
             */

            //Conserta o valor de Pos caso seja inserido algum valor que representaria as extremidades
            if (IsAtEdge(Pos) && Side == PlayerSide.Left) {
                Pos = LeftPositionRing + 2;
            } else if (IsAtEdge(Pos) && Side == PlayerSide.Right) {
                Pos = RightPositionRing - 5;
            }


            lock (SyncConsole) {
                if (!(Pos < LastPositionLeft) && Side == PlayerSide.Left)
                    if (LastPositionLeft + 3 == LastPositionRight - 2 && Pos != LastPositionLeft)
                        Pos = LastPositionLeft;
            }

            //Define as variáveis de cada linha para escrita do personagem
            string firtRow, secondRow, thirdRow;

            //Varialvel temporária que irá armazenar o valor da última posição
            int LastPos;

            //Corrige a cor a ser escrita o personagem
            if (Color == ConsoleColor.Black)
                Color = Side == PlayerSide.Left ? ColorLeft : ColorRight;

            lock (SyncConsole) {

                if (Side == PlayerSide.Left) {
                    //Define o valor temporário da variável LastPos (necessário para não haver duplicação de código)
                    LastPos = LastPositionLeft;

                    //Define os caracteres que serão escritos em cada linha 
                    firtRow = " ☻│";
                    secondRow = "/█┤";
                    thirdRow = "/ \\";

                    //Atualiza o valor da última posição do player esquerdo
                    LastPositionLeft = Pos;

                    if (EraseAtk)
                        EraseCharacter(LastPos + 2, LastPos, LastPos, 3, 4, 4);
                } else {
                    //Define o valor temporário da variável LastPos (necessário para não haver duplicação de código)
                    LastPos = LastPositionRight;

                    //Define os caracteres que serão escritos em cada linha 
                    firtRow = "│☻ ";
                    secondRow = "├█\\";
                    thirdRow = "/ \\";

                    //Atualiza o valor da última posição do player direito
                    LastPositionRight = Pos;

                    if (EraseAtk)
                        EraseCharacter(LastPos - 2, LastPos - 1, LastPos - 1, 3, 2, 4);
                }

                //Altera a cor do console conforme configurado anteriormente
                Console.ForegroundColor = Color;

                if (!EraseAtk)
                    EraseCharacter(LastPos);

                if (Player.PlayerDied == null)
                    //Escreve o personagem na posição passada como parâmetro
                    WriteCharacter(Pos, firtRow, secondRow, thirdRow);

                //Reseta as cores do Console para o padrão
                Console.ResetColor();

                if (Side == PlayerSide.Left) {
                    if (Pos + 3 == LastPositionRight - 2)
                        return 1;
                    else
                        return 0;
                } else {
                    if (Pos - 2 == LastPositionLeft + 3)
                        return 1;
                    else
                        return 0;
                }
            }
        }

        private void EraseCharacter(int FirstErase, int SecondErase, int ThirdErase, int NumFirst, int NumSecond, int NumThird) {

            if (FirstErase != -1) {
                Console.SetCursorPosition(FirstErase, HeightRing - 5);
                Console.Write(new StringBuilder().Append(' ', NumFirst).ToString());
            }

            if (SecondErase != -1) {
                Console.SetCursorPosition(SecondErase, HeightRing - 4);
                Console.Write(new StringBuilder().Append(' ', NumSecond).ToString());
            }

            if (ThirdErase != -1) {
                Console.SetCursorPosition(ThirdErase, HeightRing - 3);
                Console.Write(new StringBuilder().Append(' ', NumThird).ToString());
            }

        }

        private void EraseCharacter(int LastPos) {

            EraseCharacter(LastPos, LastPos, LastPos, 3, 4, 3);
        }

        private void WriteCharacter(int Pos, string firtRow, string secondRow, string thirdRow, int PosSecond = -1) {

            //Escreve o personagem na posição passada como parâmetro
            Console.SetCursorPosition(Pos, HeightRing - 5);
            Console.Write(firtRow);

            if (PosSecond != -1)
                Console.SetCursorPosition(PosSecond, HeightRing - 4);
            else
                Console.SetCursorPosition(Pos, HeightRing - 4);

            Console.Write(secondRow);

            Console.SetCursorPosition(Pos, HeightRing - 3);
            Console.Write(thirdRow);
        }

        public void ChiPoint(int ChiAvailable, PlayerSide Side) {
            int Pos = Side == PlayerSide.Left ? 14 : 92;

            lock (SyncConsole) {

                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(Pos, 5);
                Console.Write("     ");

                Console.SetCursorPosition(Pos, 5);

                if (Side == PlayerSide.Left)
                    Console.Write(new StringBuilder().Append('♦', ChiAvailable).ToString());
                else
                    Console.Write("{0, 6}", new StringBuilder().Append('♦', ChiAvailable).ToString());

                Console.ResetColor();
            }
        }

        public void ResizeRing(int Width) {

            string AuxLeft = new StringBuilder().Append('►', LeftPositionRing + 1).ToString();
            string AuxRight = new StringBuilder().Append('◄', WidthRing - RightPositionRing + 1).ToString();

            lock (SyncConsole) {

                Console.ForegroundColor = ConsoleColor.White;

                Task Sound = Task.Run(Console.Beep);

                for (int i = 8; i < HeightRing - 2; i++) {
                    Console.SetCursorPosition(0, i);
                    Console.Write(AuxLeft);

                    Console.SetCursorPosition(RightPositionRing - 1, i);
                    Console.Write(AuxRight);
                }

                LeftPositionRing++;
                RightPositionRing--;

                Console.ResetColor();
            }
        }

        public void WriteRing(int Life, int Chi) {

            lock (SyncConsole) {

                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(0, 0);
                Console.Write("{0}", "CONSOLE FIGHTER".PadBoth(WidthRing));

                WriteHeader(10, Life, Chi, PlayerSide.Left, NamePlayerLeft);

                WriteHeader(92, Life, Chi, PlayerSide.Right, NamePlayerRight);

                Console.ForegroundColor = ConsoleColor.White;

                for (int i = 0; i < WidthRing; i++) {
                    Console.SetCursorPosition(i, HeightRing - 23);
                    Console.Write('═');
                }

                for (int i = 8; i < HeightRing - 2; i++) {

                    Console.SetCursorPosition(120 - WidthRing, i);
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

                Console.ResetColor();
            }
        }

        private void WriteHeader(int Pos, int Life, int Atk, PlayerSide Side, string Name) {

            string firstRow, secondRow, fouthRow;

            if (Side == PlayerSide.Left) {

                firstRow = Name;
                secondRow = "VIDA";
                fouthRow = string.Format("CHI {0}", new StringBuilder().Append('♦', Atk).ToString());
            } else {

                firstRow = string.Format("{0, 10}", Name);
                secondRow = string.Format("{0, 10}", "VIDA");
                fouthRow = string.Format("{0, 6} CHI", new StringBuilder().Append('♦', Atk).ToString());
            }

            Console.ForegroundColor = Side == PlayerSide.Left ? ColorLeft : ColorRight;

            Console.SetCursorPosition(Pos, 2);
            Console.Write(firstRow);

            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(Pos, 3);
            Console.Write(secondRow);

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.SetCursorPosition(Pos, 4);
            Console.Write("{0}", new StringBuilder().Append('█', Life).ToString());

            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(Pos, 5);
            Console.Write(fouthRow);

            Console.ResetColor();

        }

        public void DeadCharacter(PlayerSide Side) {

            lock (SyncConsole) {

                EraseCharacter(LastPositionLeft, LastPositionLeft, LastPositionLeft, 5, 5, 5);
                EraseCharacter(LastPositionRight - 2, LastPositionRight - 2, LastPositionRight - 2, 5, 5, 5);

                Console.SetCursorPosition(WidthRing / 2 - 6, HeightRing / 2);

                if (Side == PlayerSide.Right) {
                    Console.ForegroundColor = ColorLeft;
                    Console.WriteLine("VOCÊ VENCEU!!");
                } else {
                    Console.ForegroundColor = ColorRight;
                    Console.WriteLine("VOCÊ PERDEU!!");
                }

                Console.ResetColor();
            }
        }

        public void ReceiveDamage(int Life, PlayerSide Side, ref int Pos, bool Especial) {

            int posInitial = Side == PlayerSide.Left ? 10 : 101;
            int posWrite;

            string WriteChar;

            if (Especial)
                Life += 2;

            for (int i = 0; i < 3; i++) {
                if (Life % 4 == 0)
                    WriteChar = " ";
                else if (Life % 4 == 1)
                    WriteChar = "░";
                else if (Life % 4 == 2)
                    WriteChar = "▒";
                else
                    WriteChar = "▓";

                lock (SyncConsole) {

                    posWrite = Side == PlayerSide.Left ? posInitial + (Life / 4) : posInitial - (Life / 4);

                    if (WriteChar == " ")
                        posWrite += Side == PlayerSide.Left ? 0 : -1;

                    Console.ForegroundColor = ConsoleColor.Cyan;

                    Console.SetCursorPosition(posWrite, 4);
                    Console.Write(WriteChar == " " ? "  " : WriteChar);

                    Console.ResetColor();

                    Life--;

                    if (i > 0)
                        continue;

                    StayNormal(ref Pos, Side, ConsoleColor.Red);
                    Thread.Sleep(50);
                    StayNormal(ref Pos, Side);
                }

                if (!Especial)
                    break;
            }
        }

        public int Menu() {
            lock (SyncConsole) {

                ConsoleKey KeySelected = ConsoleKey.F1;
                ConsoleKey Aux;

                while (true) {

                    WriteMenu(KeySelected);

                    Aux = Console.ReadKey(true).Key;

                    if (Aux == ConsoleKey.Enter
                     || Aux == ConsoleKey.F1
                     || Aux == ConsoleKey.F2
                     || Aux == ConsoleKey.F3
                     || Aux == ConsoleKey.F4
                     || Aux == ConsoleKey.F5) {

                        if (Aux != ConsoleKey.Enter) {
                            KeySelected = Aux;
                        } else {
                            if (KeySelected == ConsoleKey.F2) {
                                WriteMenuConfig();
                            } else if (KeySelected == ConsoleKey.F3) {
                                WriteMenuHelp();
                            } else if (KeySelected == ConsoleKey.F4) {
                                WriteMenuAbout();
                            }
                        }
                    } else
                        continue;

                    if (Aux == ConsoleKey.Enter && (KeySelected == ConsoleKey.F1 || KeySelected == ConsoleKey.F5))
                        return int.Parse(KeySelected.ToString().Substring(1));
                }
            }
        }

        private void WriteMenuConfig() {

            Console.Clear();

            Console.SetCursorPosition(WidthRing / 2 - 33, HeightRing / 2 - 4);
            Console.Write("Utilize as teclas de função para navegar entre as opções abaixo:");

            Console.SetCursorPosition(WidthRing / 2 - 15, HeightRing / 2 + 5);
            Console.Write("ENTER -> Salvar e voltar");

            Console.SetCursorPosition(WidthRing / 2 - 9, HeightRing / 2 + 6);
            Console.Write("ESC -> Voltar");

            ConsoleKey KeySelected = ConsoleKey.F1;
            ConsoleKey Aux = ConsoleKey.F1;

            ConsoleColor ColorLeftTemp = ColorLeft;
            ConsoleColor ColorRightTemp = ColorRight;

            string NomeLeft = NamePlayerLeft;
            string NomeRight = NamePlayerRight;

            while (Aux != ConsoleKey.Escape && Aux != ConsoleKey.Enter) {

                Console.CursorVisible = false;

                if (KeySelected == ConsoleKey.F1) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    KeySelected = ConsoleKey.F1;
                } else
                    Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(WidthRing / 2 - 33, HeightRing / 2 - 1);
                Console.Write("F1 -> Cor do Jogador(a):");

                Console.SetCursorPosition(WidthRing / 2 + 4, HeightRing / 2 - 1);
                Console.Write("[(+) Próxima (-) Anterior]");

                Console.ForegroundColor = ColorLeftTemp;

                Console.SetCursorPosition(WidthRing / 2 - 9, HeightRing / 2 - 1);
                Console.Write(ColorLeftTemp.ToString().PadBoth(13));

                if (KeySelected == ConsoleKey.F2) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    KeySelected = ConsoleKey.F2;
                } else
                    Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(WidthRing / 2 - 33, HeightRing / 2);
                Console.Write("F2 -> Cor do Computador:");

                Console.SetCursorPosition(WidthRing / 2 + 4, HeightRing / 2);
                Console.Write("[(+) Próxima (-) Anterior]");

                Console.ForegroundColor = ColorRightTemp;

                Console.SetCursorPosition(WidthRing / 2 - 9, HeightRing / 2);
                Console.Write(ColorRightTemp.ToString().PadBoth(13));

                if (KeySelected == ConsoleKey.F3) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    KeySelected = ConsoleKey.F3;
                } else
                    Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(WidthRing / 2 - 33, HeightRing / 2 + 1);
                Console.Write("F3 -> Nome do Jogador:");

                Console.SetCursorPosition(WidthRing / 2 + 4, HeightRing / 2 + 1);
                Console.Write("[(Tab) Alterar]");

                Console.SetCursorPosition(WidthRing / 2 - 10, HeightRing / 2 + 1);
                Console.Write(NomeLeft);

                if (KeySelected == ConsoleKey.F4) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    KeySelected = ConsoleKey.F4;
                } else
                    Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(WidthRing / 2 - 33, HeightRing / 2 + 2);
                Console.Write("F4 -> Nome do Computador:");

                Console.SetCursorPosition(WidthRing / 2 + 4, HeightRing / 2 + 2);
                Console.Write("[(Tab) Alterar]");

                Console.SetCursorPosition(WidthRing / 2 - 7, HeightRing / 2 + 2);
                Console.Write(NomeRight);

                Aux = Console.ReadKey(true).Key;

                if (Aux == ConsoleKey.Add || Aux == ConsoleKey.OemPlus) {
                    if (KeySelected == ConsoleKey.F1) {
                        if (!(ColorLeftTemp == ConsoleColor.White))
                            ColorLeftTemp++;
                    } else if (KeySelected == ConsoleKey.F2) {
                        if (!(ColorRightTemp == ConsoleColor.White))
                            ColorRightTemp++;
                    }
                } else if (Aux == ConsoleKey.Subtract || Aux == ConsoleKey.OemMinus) {
                    if (KeySelected == ConsoleKey.F1) {
                        if (!(ColorLeftTemp == ConsoleColor.Black))
                            ColorLeftTemp--;
                    } else if (KeySelected == ConsoleKey.F2) {
                        if (!(ColorRightTemp == ConsoleColor.Black))
                            ColorRightTemp--;
                    }
                } else if (Aux == ConsoleKey.F1 || Aux == ConsoleKey.F2 || Aux == ConsoleKey.F3 || Aux == ConsoleKey.F4) {
                    KeySelected = Aux;
                } else if (Aux == ConsoleKey.Tab) {
                    if (KeySelected == ConsoleKey.F3) {
                        Console.SetCursorPosition(WidthRing / 2 - 10, HeightRing / 2 + 1);
                        Console.Write(new StringBuilder().Append(' ', 10).ToString());
                        Console.SetCursorPosition(WidthRing / 2 - 10, HeightRing / 2 + 1);

                        Console.CursorVisible = true;

                        NomeLeft = Console.ReadLine();

                        Console.SetCursorPosition(WidthRing / 2 + 19, HeightRing / 2 + 1);
                    } else if (KeySelected == ConsoleKey.F4) {
                        Console.SetCursorPosition(WidthRing / 2 - 7, HeightRing / 2 + 2);
                        Console.Write(new StringBuilder().Append(' ', 10).ToString());
                        Console.SetCursorPosition(WidthRing / 2 - 7, HeightRing / 2 + 2);

                        Console.CursorVisible = true;

                        NomeRight = Console.ReadLine();

                        Console.SetCursorPosition(WidthRing / 2 + 19, HeightRing / 2 + 2);
                    }
                }
            }

            if (Aux == ConsoleKey.Enter) {
                ColorRight = ColorRightTemp;
                ColorLeft = ColorLeftTemp;
                NamePlayerLeft = NomeLeft;
                NamePlayerRight = NomeRight;
            }
        }

        private bool WriteMenuHelp() {

            int Pos = 15;

            Console.Clear();

            RestartValues();

            WriteRing(10, 1);

            StayNormal(ref Pos, PlayerSide.Left);

            Pos = 102;

            StayNormal(ref Pos, PlayerSide.Right);

            Console.ForegroundColor = ConsoleColor.Red;

            Console.SetCursorPosition(WidthRing / 2 - 10, HeightRing / 2 - 1);
            Console.Write("P -> Próximo Passo");

            Console.SetCursorPosition(WidthRing / 2 - 10, HeightRing / 2);
            Console.Write("ESC -> Cancelar");

            Console.ForegroundColor = ConsoleColor.Yellow;

            ConsoleKey Aux;
            int Step = 1;

            while (true) {

                Aux = Console.ReadKey(true).Key;

                if (Aux == ConsoleKey.P) {

                    if (Step == 6)
                        return true;

                    if (Step == 1) {
                        Console.SetCursorPosition(22, 4);
                        Console.Write("-> Barra de Vida: Vai sumindo a medida que recebe dano.");

                        Step++;
                    } else if (Step == 2) {
                        Console.SetCursorPosition(22, 5);
                        Console.Write("-> Chi: Acima de 2 pontos, pressione 'P' para dar um ataque com");

                        Console.SetCursorPosition(22, 6);
                        Console.Write("3 de dano. Quebra a defesa do adversário e arremesse-o para trás.");

                        Step++;
                    } else if (Step == 3) {
                        Console.SetCursorPosition(3, 8);
                        Console.Write("-> Espinhos: CUIDADO! A cada 1s os espinhos irão crescer, se ficar muito próximo a eles, poderá ser atingido.");

                        Step++;
                    } else if (Step == 4) {

                        Console.SetCursorPosition(10, HeightRing - 9);
                        Console.Write("Seu Personagem");

                        Console.SetCursorPosition(16, HeightRing - 8);
                        Console.Write("▼");

                        Console.SetCursorPosition(20, HeightRing - 7);
                        Console.Write("'A' -> Mover para trás");

                        Console.SetCursorPosition(20, HeightRing - 6);
                        Console.Write("'D' -> Mover para frente");

                        Console.SetCursorPosition(20, HeightRing - 5);
                        Console.Write("[ENTER] -> Atacar");

                        Console.SetCursorPosition(20, HeightRing - 4);
                        Console.Write("[ESPAÇO] -> Defender");

                        Console.SetCursorPosition(20, HeightRing - 3);
                        Console.Write("'P' -> Ataque especial");

                        Step++;
                    } else if (Step == 5) {
                        Console.SetCursorPosition(99, HeightRing - 8);
                        Console.Write("Computador");

                        Console.SetCursorPosition(103, HeightRing - 7);
                        Console.Write("▼");

                        Step++;
                    }
                } else if (Aux == ConsoleKey.Escape)
                    return false;
            }
        }

        private void WriteMenuAbout() {

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(0, HeightRing / 2 - 6);
            Console.Write("AUTORES".PadBoth(WidthRing));

            Console.SetCursorPosition(0, HeightRing / 2 - 4);
            Console.Write("Carlos Eduardo".PadBoth(WidthRing));

            Console.SetCursorPosition(0, HeightRing / 2 - 3);
            Console.Write("Junio Almeida".PadBoth(WidthRing));

            Console.SetCursorPosition(0, HeightRing / 2 - 2);
            Console.Write("Matheus Douglas".PadBoth(WidthRing));

            Console.SetCursorPosition(0, HeightRing / 2 - 1);
            Console.Write("Nathan Nascimento".PadBoth(WidthRing));

            Console.SetCursorPosition(0, HeightRing / 2 + 1);
            Console.Write("VERSÃO".PadBoth(WidthRing));

            Console.SetCursorPosition(0, HeightRing / 2 + 2);
            Console.Write("1.0".PadBoth(WidthRing));

            Console.SetCursorPosition(0, HeightRing / 2 + 4);
            Console.Write("ESC -> Voltar".PadBoth(WidthRing));

            ConsoleKey Wait;

            do {
                Wait = Console.ReadKey(true).Key;
            } while (Wait != ConsoleKey.Escape);

        }

        private void WriteMenu(ConsoleKey Key) {

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(WidthRing / 2 - 33, HeightRing / 2 - 4);
            Console.Write("Utilize as teclas de função para navegar entre as opções abaixo: ");

            if (Key == ConsoleKey.F1)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(WidthRing / 2 - 8, HeightRing / 2 - 2);
            Console.Write("F1 -> JOGAR");

            if (Key == ConsoleKey.F2)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(WidthRing / 2 - 8, HeightRing / 2 - 1);
            Console.Write("F2 -> OPÇÕES");

            if (Key == ConsoleKey.F3)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(WidthRing / 2 - 8, HeightRing / 2);
            Console.Write("F3 -> AJUDA");

            if (Key == ConsoleKey.F4)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(WidthRing / 2 - 8, HeightRing / 2 + 1);
            Console.Write("F4 -> SOBRE");

            if (Key == ConsoleKey.F5)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(WidthRing / 2 - 8, HeightRing / 2 + 2);
            Console.Write("F5 -> SAIR");
        }
    }
}
