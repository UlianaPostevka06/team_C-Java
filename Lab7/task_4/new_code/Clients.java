package variant6_ecommercepaymentterminal;

import java.math.BigDecimal;
import java.util.Objects;
import variant6_ecommercepaymentterminal.PaymentInterfaces.*;

/** Клієнти: залежать лише від того інтерфейсу, який викликають. */
public final class Clients {
    private Clients() {}

    public static class CardCheckoutService {
        private final CardPayment payment;

        public CardCheckoutService(CardPayment payment) {
            this.payment = Objects.requireNonNull(payment);
        }

        public void pay(BigDecimal amount, String cardNumber) {
            payment.processCreditCard(amount, cardNumber);
        }
    }

    public static class ReceiptService {
        private final ReceiptPrinter printer;

        public ReceiptService(ReceiptPrinter printer) {
            this.printer = Objects.requireNonNull(printer);
        }

        public void print(String text) {
            printer.printPaperReceipt(text);
        }
    }
}
