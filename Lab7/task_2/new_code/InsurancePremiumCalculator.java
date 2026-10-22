package variant6_insurancepremiumcalculator;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.util.EnumMap;
import java.util.List;
import java.util.Map;
import java.util.Objects;


interface PremiumStrategy {
    /** Тип полісу, який обслуговує ця стратегія. */
    PolicyType supportedType();

    BigDecimal calculate(PolicyHolder holder);
}

interface PremiumLogger {
    void log(String message);
}

public final class InsurancePremiumCalculator {
    private static final int SCALE = 2;

    private final Map<PolicyType, PremiumStrategy> strategies = new EnumMap<>(PolicyType.class);
    private final PremiumLogger logger;

    InsurancePremiumCalculator(List<PremiumStrategy> strategies, PremiumLogger logger) {
        Objects.requireNonNull(strategies, "strategies не може бути null");
        this.logger = Objects.requireNonNull(logger, "logger не може бути null");
        for (PremiumStrategy strategy : strategies) {
            PremiumStrategy previous = this.strategies.put(strategy.supportedType(), strategy);
            if (previous != null) {
                throw new IllegalArgumentException(
                        "Дублювання стратегії для типу " + strategy.supportedType());
            }
        }
    }

    public BigDecimal calculatePremium(PolicyHolder holder, PolicyType type) {
        Objects.requireNonNull(holder, "holder не може бути null");
        PremiumStrategy strategy = strategies.get(type);
        if (strategy == null) {
            throw new IllegalArgumentException("Тип страхування " + type + " не зареєстровано у системі.");
        }

        BigDecimal premium = strategy.calculate(holder).setScale(SCALE, RoundingMode.HALF_UP);
        logger.log("[INSURANCE] Клієнт " + holder.getHolderId() + " (Вік: " + holder.getAge()
                + "): Страховий поліс " + type + " -> Річна премія: $" + premium);
        return premium;
    }
}
