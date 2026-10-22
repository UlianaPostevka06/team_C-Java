package variant6_ecommercepaymentterminal;

import java.math.BigDecimal;

// Опис предметної області: Програмно-апаратні платіжні термінали та касові системи.
// Завдання коду: Проведення транзакцій картками, NFC, банківськими переказами, подарунковими сертифікатами та друк паперових чеків.

public class ECommercePaymentTerminal {

    public interface IPaymentTerminal {
        void processCreditCard(BigDecimal amount, String cardNumber);
        void processNfcTap(BigDecimal amount);
        void processBankTransfer(BigDecimal amount, String iban);
        void processGiftCard(BigDecimal amount, String voucherCode);
        void printPaperReceipt(String text);
    }

    public static class OnlineWebCheckoutTerminal implements IPaymentTerminal {
        @Override public void processCreditCard(BigDecimal amount, String cardNumber) { System.out.println("Web charged " + amount + " via card."); }
        @Override public void processBankTransfer(BigDecimal amount, String iban) { System.out.println("Web charged " + amount + " via IBAN."); }

        @Override public void processNfcTap(BigDecimal amount) { throw new UnsupportedOperationException("Web browser checkout cannot scan physical NFC cards."); }
        @Override public void processGiftCard(BigDecimal amount, String voucherCode) { throw new UnsupportedOperationException("Gift cards not supported on web checkout."); }
        @Override public void printPaperReceipt(String text) { throw new UnsupportedOperationException("Web checkout cannot print physical paper receipts."); }
    }

    public static class PosCardReader implements IPaymentTerminal {
        @Override public void processCreditCard(BigDecimal amount, String cardNumber) { System.out.println("POS processed card " + amount + "."); }
        @Override public void processNfcTap(BigDecimal amount) { System.out.println("POS read NFC tap " + amount + "."); }
        @Override public void printPaperReceipt(String text) { System.out.println("Printed receipt: " + text); }

        @Override public void processBankTransfer(BigDecimal amount, String iban) { throw new UnsupportedOperationException("POS terminal does not process wire transfers."); }
        @Override public void processGiftCard(BigDecimal amount, String voucherCode) { throw new UnsupportedOperationException("Vouchers not processed by POS card reader."); }
    }
}