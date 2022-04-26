using System;

namespace ConsoleFighter {

    public enum PlayerSide {
        Left,
        Right
    }

    public interface IWritter {

        void WriteRing(int Life, int Chi);
        void ResizeRing(int Width);
        void Attack(int Pos, PlayerSide Side);
        void Defend(int Pos, PlayerSide Side);
        int StayNormal(ref int Pos, PlayerSide Side, ConsoleColor Color = ConsoleColor.Black, bool EraseAtkDef = false);
        void ChiPoint(int ChiAvailable, PlayerSide Side);
        void DeadCharacter(PlayerSide Side);
        void ReceiveDamage(int Life, PlayerSide Side, ref int Pos, bool Especial);
        bool IsAtEdge(int Pos);
        int Menu();
        void RestartValues();

    }
}
