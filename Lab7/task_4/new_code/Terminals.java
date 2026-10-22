package variant6_ecommercepaymentterminal;

import java.math.BigDecimal;
import variant6_ecommercepaymentterminal.PaymentInterfaces.*;

/** Термінали: кожен реалізує лише ті ролі, які справді підтримує. */
public final class Terminals {
    private Terminals() {}

    public static class OnlineWebCheckoutTerminal implements CardPayment, BankTransferPayment {
        @Override public void processCreditCard(BigDecimal amount, String cardNumber) { System.out.println("Web charged " + amount + " via card."); }
        @Override public void processBankTransfer(BigDecimal amount, String iban) { System.out.println("Web charged " + amount + " via IBAN."); }
    }

    public static class PosCardReader implements CardPayment, NfcPayment, ReceiptPrinter {
        @Override public void processCreditCard(BigDecimal amount, String cardNumber) { System.out.println("POS processed card " + amount + "."); }
        @Override public void processNfcTap(BigDecimal amount) { System.out.println("POS read NFC tap " + amount + "."); }
        @Override public void printPaperReceipt(String text) { System.out.println("Printed receipt: " + text); }
    }
}
