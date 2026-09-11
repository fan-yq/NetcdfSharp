// NetcdfSharp.org.apache.commons.math3.stat.descriptive.SummaryStatistics shim.
// Minimal subset: addValue/getMin/getMax/getMean/getStandardDeviation.
using System;
using System.Collections.Generic;

namespace NetcdfSharp.org.apache.commons.math3.stat.descriptive
{
    /// <summary>Computes summary statistics for a stream of data values. Minimal subset.</summary>
    public class SummaryStatistics
    {
        private long _n;
        private double _min = double.PositiveInfinity;
        private double _max = double.NegativeInfinity;
        private double _sum;
        private double _sumSq;

        public virtual void addValue(double value)
        {
            _n++;
            _min = Math.Min(_min, value);
            _max = Math.Max(_max, value);
            _sum += value;
            _sumSq += value * value;
        }

        public virtual void clear()
        {
            _n = 0; _min = double.PositiveInfinity; _max = double.NegativeInfinity; _sum = 0; _sumSq = 0;
        }

        public virtual long getN() => _n;
        public virtual double getMin() => _min;
        public virtual double getMax() => _max;
        public virtual double getSum() => _sum;
        public virtual double getMean() => _n == 0 ? 0 : _sum / _n;

        public virtual double getStandardDeviation()
        {
            if (_n < 2) return 0;
            double mean = getMean();
            double variance = (_sumSq - mean * mean * _n) / (_n - 1);
            return variance < 0 ? 0 : Math.Sqrt(variance);
        }

        public virtual double getVariance()
        {
            if (_n < 2) return 0;
            double mean = getMean();
            double variance = (_sumSq - mean * mean * _n) / (_n - 1);
            return variance < 0 ? 0 : variance;
        }
    }
}
