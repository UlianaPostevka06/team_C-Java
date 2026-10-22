package variant6_insurancepremiumcalculator;

import java.math.BigDecimal;

/** Автострахування: 3% від суми; +20%, якщо водію менше 25 років або були страхові випадки. */
final class AutoPremiumStrategy implements PremiumStrategy {
    private static final BigDecimal RATE = new BigDecimal("0.03");
    private static final BigDecimal RISK_SURCHARGE = new BigDecimal("1.20");
    private static final int YOUNG_DRIVER_AGE_LIMIT = 25;

    @Override
    public PolicyType supportedType() {
        return PolicyType.AUTO;
    }

    @Override
    public BigDecimal calculate(PolicyHolder holder) {
        BigDecimal premium = holder.getInsuredAmount().multiply(RATE);
        boolean highRisk = holder.getAge() < YOUNG_DRIVER_AGE_LIMIT || holder.hasPriorClaims();
        return highRisk ? premium.multiply(RISK_SURCHARGE) : premium;
    }
}

/** Медичне страхування: $200 базово + $10 за кожен рік після 40 років + $150 за попередні випадки. */
final class HealthPremiumStrategy implements PremiumStrategy {
    private static final BigDecimal BASE = new BigDecimal("200.00");
    private static final BigDecimal PER_YEAR_OVER_THRESHOLD = new BigDecimal("10.00");
    private static final BigDecimal PRIOR_CLAIMS_FEE = new BigDecimal("150.00");
    private static final int AGE_THRESHOLD = 40;

    @Override
    public PolicyType supportedType() {
        return PolicyType.HEALTH;
    }

    @Override
    public BigDecimal calculate(PolicyHolder holder) {
        int extraYears = Math.max(0, holder.getAge() - AGE_THRESHOLD);
        BigDecimal premium = BASE.add(PER_YEAR_OVER_THRESHOLD.multiply(BigDecimal.valueOf(extraYears)));
        return holder.hasPriorClaims() ? premium.add(PRIOR_CLAIMS_FEE) : premium;
    }
}

/** Страхування майна: 0.5% від оціночної вартості нерухомості. */
final class PropertyPremiumStrategy implements PremiumStrategy {
    private static final BigDecimal RATE = new BigDecimal("0.005");

    @Override
    public PolicyType supportedType() {
        return PolicyType.PROPERTY;
    }

    @Override
    public BigDecimal calculate(PolicyHolder holder) {
        return holder.getInsuredAmount().multiply(RATE);
    }
}

/** Туристичне страхування: фіксований поліс $30; +50% для осіб віком 65+. */
final class TravelPremiumStrategy implements PremiumStrategy {
    private static final BigDecimal BASE = new BigDecimal("30.00");
    private static final BigDecimal SENIOR_SURCHARGE = new BigDecimal("1.50");
    private static final int SENIOR_AGE = 65;

    @Override
    public PolicyType supportedType() {
        return PolicyType.TRAVEL;
    }

    @Override
    public BigDecimal calculate(PolicyHolder holder) {
        return holder.getAge() >= SENIOR_AGE ? BASE.multiply(SENIOR_SURCHARGE) : BASE;
    }
}

/**
 * Правило (умовне): 1% від суми + $5 за кожен рік після 50 років.
 */
final class LifePremiumStrategy implements PremiumStrategy {
    private static final BigDecimal RATE = new BigDecimal("0.01");
    private static final BigDecimal PER_YEAR_OVER_THRESHOLD = new BigDecimal("5.00");
    private static final int AGE_THRESHOLD = 50;

    @Override
    public PolicyType supportedType() {
        return PolicyType.LIFE;
    }

    @Override
    public BigDecimal calculate(PolicyHolder holder) {
        int extraYears = Math.max(0, holder.getAge() - AGE_THRESHOLD);
        return holder.getInsuredAmount().multiply(RATE)
                .add(PER_YEAR_OVER_THRESHOLD.multiply(BigDecimal.valueOf(extraYears)));
    }
}
