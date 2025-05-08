using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Engine;

namespace Game
{
    /* Set L/R Shoulder and CurrentValue, do not use GenerateRandomly if Range Goes beyond 100 */
    /* Defuzzify using either First of Max / Fuzzy Mean / Adaptive Integration (recommended) / Influence Value / constraintDecision */
    public class Fuzz
    {
        protected int HighValueSt, HighValueEd;
        protected int LowValueSt, LowValueEd;

        public float CurrentValue = 0.0f;

        public Fuzz() { }
        public Fuzz(int Lowst, int LowEd, int Highst, int HighEd)
        {
            HighValueSt = Highst; HighValueEd = HighEd;
            LowValueSt = Lowst; LowValueEd = LowEd;
        }
        public void ResetValue(int Lowst, int LowEd, int Highst, int HighEd) 
        {
            HighValueSt = Highst; HighValueEd = HighEd;
            LowValueSt = Lowst; LowValueEd = LowEd;
        }
        public void GenerateRandom()
        {
            LowValueSt = GameEngine.RandomValue.Next(12, 25);
            LowValueEd = LowValueSt + GameEngine.RandomValue.Next(10, 25);

            HighValueSt = GameEngine.RandomValue.Next(50, 75);
            HighValueEd = HighValueSt + GameEngine.RandomValue.Next(10, 25);
        }
        public float HowHigh()
        {
        	if(CurrentValue <= HighValueSt) return 0.0f;
            if(CurrentValue >= HighValueEd) return 1.0f;

            return (float)(CurrentValue - HighValueSt) / (HighValueEd - HighValueSt);
        }
        public float HowLow()
        {
            if(CurrentValue >= LowValueEd) return 0.0f;
	        if(CurrentValue <= LowValueSt) return 1.0f;

	        return (float)(LowValueEd - CurrentValue) / (float)(LowValueEd - LowValueSt);
        }
    }
}

