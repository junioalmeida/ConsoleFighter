using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleFighter {
    class Player {

        private readonly object SyncAtkDefMove = new object();

        public int HealthPoints { get; private set; }
        private int _Position;

        public int Position {
            get {
                lock (SyncAtkDefMove)
                    return _Position;
            }
            private set { _Position = value; }
        }

        public bool DefendingState { get; private set; }
        public int AtkAvailable { get; set; }
        public int DefAvailable { get; private set; }

        private PlayerSide Side;

        private IWritter Write;

        private Thread AttackThread;
        private Thread DefenseThread;

        public Player(int initialPosition, PlayerSide Side) {
            Write = Writter.GetInstance();

            HealthPoints = 9;
            Position = initialPosition;
            AtkAvailable = 3;
            DefAvailable = 2;
            DefendingState = false;
            this.Side = Side;

            AttackThread = new Thread(GenerateAtk);
            DefenseThread = new Thread(GenerateDef);

            AttackThread.Name = "GenarateAtk";
            DefenseThread.Name = "GenerateDef";

            AttackThread.Start();
            DefenseThread.Start();

            Write.StayNormal(ref _Position, Side);
        }

        public void Move(bool ToRight) {
            lock (SyncAtkDefMove) {
                if (ToRight)
                    Position++;
                else
                    Position--;

                Write.StayNormal(ref _Position, Side);
            }
        }

        public void Attack() {

            bool lockTaken = false;

            try {
                Monitor.Enter(SyncAtkDefMove, ref lockTaken);

                if (AtkAvailable > 0) {

                    AtkAvailable--;

                    Write.Attack();
                    Write.SubtractAttackPoint();

                    // ---------------------------------
                    // <<< Dar dano no outro jogador >>>
                    // ---------------------------------

                    Write.StayNormal(ref _Position, Side);
                }

                Monitor.PulseAll(SyncAtkDefMove);

            } finally {
                if (lockTaken)
                    Monitor.Exit(SyncAtkDefMove);
            }
        }

        public void Defend() {

            bool lockTaken = false;

            try {

                Monitor.Enter(SyncAtkDefMove, ref lockTaken);

                if (DefAvailable > 0) {

                    DefAvailable--;
                    DefendingState = true;

                    Write.Defend();
                    Write.SubtractDefensePoint();

                    Thread.Sleep(200);

                    DefendingState = false;
                    Write.StayNormal(ref _Position, Side);
                }

                Monitor.PulseAll(SyncAtkDefMove);

            } finally {
                if (lockTaken)
                    Monitor.Exit(SyncAtkDefMove);
            }

        }
        private void GenerateAtk() {

            bool lockTaken;

            while (true) {

                lockTaken = false;

                try {

                    Monitor.Enter(SyncAtkDefMove, ref lockTaken);

                    if (!(AtkAvailable >= 6)) {
                        AtkAvailable++;
                        Write.AddAttackPoint();

                        Monitor.Exit(SyncAtkDefMove);
                        lockTaken = false;

                        Thread.Sleep(3000);
                    } else {
                        while (AtkAvailable >= 6)
                            Monitor.Wait(SyncAtkDefMove);
                    }

                } finally {
                    if (lockTaken)
                        Monitor.Exit(SyncAtkDefMove);
                }
            }
        }

        private void GenerateDef() {

            bool lockTaken;

            while (true) {

                lockTaken = false;

                try {
                    Monitor.Enter(SyncAtkDefMove, ref lockTaken);

                    if (!(DefAvailable >= 6)) {

                        DefAvailable++;
                        Write.AddDefensePoint();

                        Monitor.Exit(SyncAtkDefMove);
                        lockTaken = false;

                        Thread.Sleep(500);
                    } else {

                        while (DefAvailable >= 6)
                            Monitor.Wait(SyncAtkDefMove);
                    }

                } finally {
                    if (lockTaken)
                        Monitor.Exit(SyncAtkDefMove);
                }
            }
        }

        public void ReceiveDamage(int AmountDamege) {
            HealthPoints -= AmountDamege;
            Write.SubtractLife();

            if (HealthPoints <= 0) {
                throw new DeadCharacterException(this);
            }
        }
    }
}
