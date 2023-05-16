using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo
{
    [Indicator(AccessRights = AccessRights.None, IsOverlay = false)]
    public class RealRelativeStrength : Indicator
    {
        [Parameter(DefaultValue = "AUDNZD")]
        public string ComparedSymbol { get; set; }

        [Parameter("Lookback", DefaultValue = 10, MaxValue = 100, MinValue = 1, Step = 1)]
        public int Lookback { get; set; }

        [Parameter("Smooth", DefaultValue = 3, MaxValue = 100, MinValue = 1, Step = 1)]
        public int Smooth { get; set; }

        [Output("Real Relative Strength", LineColor = "Red")]
        public IndicatorDataSeries RRS { get; set; }


        private Bars _comparedBars;
        private IndicatorDataSeries _relativeStrength;
        private MovingAverage _maRelativeStrength;
        
        private AverageTrueRange _comparedATR;
        private AverageTrueRange _symbolATR;

        private double comparedRollingMove;
        private double symbolRollingMove;
        protected override void Initialize()
        {
            // To learn more about cTrader Automate visit our Help Center:
            // https://help.ctrader.com/ctrader-automate
            _comparedBars = MarketData.GetBars(Bars.TimeFrame, ComparedSymbol);
            _comparedATR = Indicators.AverageTrueRange(_comparedBars, Lookback, MovingAverageType.Simple);

            _symbolATR = Indicators.AverageTrueRange(Lookback, MovingAverageType.Simple);
            _maRelativeStrength = Indicators.MovingAverage(_relativeStrength, Smooth, MovingAverageType.Simple);
        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index
            // Result[index] =
            comparedRollingMove = _comparedBars.ClosePrices.Last(0) - _comparedBars.ClosePrices.Last(Lookback);
            symbolRollingMove = Bars.ClosePrices.Last() - Bars.ClosePrices.Last(Lookback);

            _relativeStrength[index] = (symbolRollingMove / _symbolATR.Result.Last()) - (comparedRollingMove / _comparedATR.Result.Last());

            RRS[index] = _maRelativeStrength.Result.Last();

        }
    }
}