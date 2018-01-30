﻿using System;
using System.Collections.Generic;

namespace ReplaceConditionalLogicWithStrategy.MyWork
{
    public class Loan
    {
        double _commitment = 1.0;
        private DateTime? _expiry;
        private DateTime? _maturity;
        private double _outstanding;
        IList<Payment> _payments = new List<Payment>();
        private DateTime? _today = DateTime.Now;
        private DateTime _start;
        private long MILLIS_PER_DAY = 86400000;
        private long DAYS_PER_YEAR = 365;
        private double _riskRating;
        private double _unusedPercentage;
        private CapitalStrategy _capitalStrategy;

        public Loan(double commitment, 
                    double notSureWhatThisIs, 
                    DateTime start, 
                    DateTime? expiry, 
                    DateTime? maturity, 
                    int riskRating,
                    CapitalStrategy capitalStrategy)
        {
            this._expiry = expiry;
            this._commitment = commitment;
            this._today = null;
            this._start = start;
            this._maturity = maturity;
            this._riskRating = riskRating;
            this._unusedPercentage = 1.0;
            this._capitalStrategy = capitalStrategy;
        }

        public static Loan NewTermLoan(double commitment, DateTime start, DateTime maturity, int riskRating)
        {
            return new Loan(commitment, commitment, start, null, 
                            maturity, riskRating, new CapitalStrategyTermLoan());
        }

        public static Loan NewRevolver(double commitment, DateTime start, DateTime expiry, int riskRating) 
        {
            return new Loan(commitment, 0, start, expiry,
                            null, riskRating, new CapitalStrategy());
        }

        public static Loan NewAdvisedLine(double commitment, DateTime start, DateTime expiry, int riskRating)
        {
            if (riskRating > 3) return null;
            Loan advisedLine = new Loan(commitment, 0, start, expiry,
                                        null, riskRating, new CapitalStrategy());
            advisedLine.SetUnusedPercentage(0.1);
            return advisedLine;
        }

        public DateTime? GetExpiry()
        {
            return _expiry;
        }

        public double GetCommitment()
        {
            return _commitment;
        }

        public DateTime? GetMaturity()
        {
            return _maturity;
        }

        public double GetRiskRating()
        {
            return _riskRating;
        }

        public void Payment(double amount, DateTime paymentDate)
        {
            _payments.Add(new Payment(amount, paymentDate));
        }

        public double Capital() {
            return _capitalStrategy.Capital(this);
        }

        public DateTime? GetToday()
        {
            return _today;
        }

        public DateTime? GetStart()
        {
            return _start;
        }

        public double Duration()
        {
            if (_expiry == null && _maturity != null)
            {
                return WeightedAverageDuration();
            }
            else if (_expiry != null && _maturity == null)
            {
                return YearsTo(_expiry);
            }
            return 0.0;
        }

        private double WeightedAverageDuration()
        { 
            double duration = 0.0;
            double weightedAverage = 0.0;
            double sumOfPayments = 0.0;

            foreach (var payment in _payments)
            {
                sumOfPayments += payment.Amount;
                weightedAverage += YearsTo(payment.Date) * payment.Amount;
            }

            if (_commitment != 0.0)
            {
                duration = weightedAverage / sumOfPayments;
            }

            return duration;
        }

        public IList<Payment> Payments() {
            return _payments;
        }

        private double YearsTo(DateTime? endDate)
        {
            DateTime? beginDate = (_today == null ? _start : _today);
            return (double)((endDate?.Ticks - beginDate?.Ticks) / MILLIS_PER_DAY / DAYS_PER_YEAR);
        }

       

        public double GetUnusedPercentage()
        {
            return _unusedPercentage;
        }

        public void SetUnusedPercentage(double unusedPercentage) 
        {
            _unusedPercentage = unusedPercentage;
        }

        public double UnusedRiskAmount()
        {
            return (_commitment - _outstanding);
        }

        public double OutstandingRiskAmount()
        {
            return _outstanding;
        }
    }
}