﻿namespace MoneyFox.Foundation
{
    public enum PaymentType
    {
        Expense,
        Income,
        Transfer
    }

    public enum PaymentRecurrence
    {
        Daily = 0,
        DailyWithoutWeekend = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4,
        Biweekly = 5,
        Bimonthly = 6,
        Quarterly = 7,
        Biannually = 8
    }

    public enum StatisticType
    {
        Cashflow,
        CategorySpreading,
        CategorySummary,
        CashflowHistory
    }
    public enum SettingsType
    {
        Categories,
        Backup,
        About
    }

    public enum TyleType
    {
        Income,
        Expense,
        Transfer
    }

    public enum ModifyOperation
    {
        Edit,
        Delete
    }

    public enum AppTheme
    {
        Dark,
        Light
    }

    public enum AppPlatform
    {
        XamarinForms,
        UWP
    }
}