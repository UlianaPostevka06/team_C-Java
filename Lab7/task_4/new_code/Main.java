package variant6_ecommercepaymentterminal;

import java.math.BigDecimal;
import variant6_ecommercepaymentterminal.Clients.*;
import variant6_ecommercepaymentterminal.Terminals.*;

/** Composition Root: тут збираються залежності. */
public class Main {
    public static void main(String[] args) {
        OnlineWebCheckoutTerminal web = new OnlineWebCheckoutTerminal();
        PosCardReader pos = new PosCardReader();

        new CardCheckoutService(web).pay(new BigDecimal("100.00"), "4111111111111111");
        new CardCheckoutService(pos).pay(new BigDecimal("100.00"), "4111111111111111");
        new ReceiptService(pos).print("Total: 100.00");

        // new ReceiptService(web);  // помилка компіляції: web не реалізує ReceiptPrinter
    }
}
