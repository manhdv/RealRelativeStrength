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
    [Levels(4, 0, -4)]
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

        public IndicatorDataSeries LowerBand { get; set; }

        private Bars _comparedBars;
        private IndicatorDataSeries _relativeStrength;
        private MovingAverage _maRelativeStrength;
        
        private AverageTrueRange _comparedATR;
        private AverageTrueRange _symbolATR;

        private double _comparedRollingMove;
        private double _symbolRollingMove;
        protected override void Initialize()
        {
            // To learn more about cTrader Automate visit our Help Center:
            // https://help.ctrader.com/ctrader-automate
             _relativeStrength = CreateDataSeries();
            _maRelativeStrength = Indicators.MovingAverage(_relativeStrength, Smooth, MovingAverageType.Simple);


            _comparedBars = MarketData.GetBars(Bars.TimeFrame, ComparedSymbol);
            _comparedATR = Indicators.AverageTrueRange(_comparedBars, Lookback, MovingAverageType.Simple);

            _symbolATR = Indicators.AverageTrueRange(Lookback, MovingAverageType.Simple);
        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index
            // Result[index] =
            if(index > Lookback)
            {
                _comparedRollingMove = _comparedBars.ClosePrices[index] - _comparedBars.ClosePrices[index - Lookback];
                _symbolRollingMove = Bars.ClosePrices[index] - Bars.ClosePrices[index - Lookback];

                _relativeStrength[index] = (_symbolRollingMove / _symbolATR.Result[index - 1]) - (_comparedRollingMove / _comparedATR.Result[index-1]);
            }
            else
            {
                _relativeStrength[index] = 0;
            }

            RRS[index] = _maRelativeStrength.Result.Last();

        }
    }
}