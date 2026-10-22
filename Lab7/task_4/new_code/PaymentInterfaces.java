package variant6_ecommercepaymentterminal;

import java.math.BigDecimal;

/** Role-based інтерфейси (ISP): кожен описує одну роль. */
public final class PaymentInterfaces {
    private PaymentInterfaces() {}

    public interface CardPayment {
        void processCreditCard(BigDecimal amount, String cardNumber);
    }
    public interface NfcPayment {
        void processNfcTap(BigDecimal amount);
    }
    public interface BankTransferPayment {
        void processBankTransfer(BigDecimal amount, String iban);
    }
    public interface GiftCardPayment {
        void processGiftCard(BigDecimal amount, String voucherCode);
    }
    public interface ReceiptPrinter {
        void printPaperReceipt(String text);
    }
}
