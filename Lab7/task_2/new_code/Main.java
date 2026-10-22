package variant6_insurancepremiumcalculator;

import java.math.BigDecimal;
import java.util.List;

/** Реалізація логера для production: вивід у консоль. */
final class ConsolePremiumLogger implements PremiumLogger {
    @Override
    public void log(String message) {
        System.out.println(message);
    }
}

/** Composition Root: єдине місце, де створюються конкретні залежності. */
public final class Main {
    public static void main(String[] args) {

        InsurancePremiumCalculator calculator = new InsurancePremiumCalculator(
                List.of(new AutoPremiumStrategy(), new HealthPremiumStrategy(),
                        new PropertyPremiumStrategy(), new TravelPremiumStrategy()),
                new ConsolePremiumLogger());

        PolicyHolder young = new PolicyHolder("C-001", 22, new BigDecimal("20000"), false);
        PolicyHolder senior = new PolicyHolder("C-002", 67, new BigDecimal("150000"), true);

        calculator.calculatePremium(young, PolicyType.AUTO);
        calculator.calculatePremium(senior, PolicyType.HEALTH);
        calculator.calculatePremium(senior, PolicyType.PROPERTY);
        calculator.calculatePremium(senior, PolicyType.TRAVEL);

        try {
            calculator.calculatePremium(senior, PolicyType.LIFE);
        } catch (IllegalArgumentException e) {
            System.out.println("[EXPECTED ERROR] " + e.getMessage());
        }

        InsurancePremiumCalculator extended = new InsurancePremiumCalculator(
                List.of(new AutoPremiumStrategy(), new HealthPremiumStrategy(),
                        new PropertyPremiumStrategy(), new TravelPremiumStrategy(),
                        new LifePremiumStrategy()),
                new ConsolePremiumLogger());
        extended.calculatePremium(senior, PolicyType.LIFE);
    }
}
