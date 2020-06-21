using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFighter {

    public enum PlayerSide {
        Left,
        Right
    }

    public interface IWritter {

        void WriteRing();
        void Attack();
        void Defend();
        void StayNormal(ref int Pos, PlayerSide Side);
        void AddAttackPoint();
        void SubtractAttackPoint();
        void AddDefensePoint();
        void SubtractDefensePoint();
        void SubtractLife();
        void DeadCharacter();
    }
}
