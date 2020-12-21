module.exports = {
  "Booking": {
    "Confirm your booking": {
      "1": "\n                    \n<form action=\"/ui-booking/page-three/check-your-booking\" autocomplete=\"on\" enctype=\"multipart/form-data\" method=\"post\" novalidate=\"novalidate\">\n<input id=\"booking-appointment-date\" name=\"booking-appointment-date\" type=\"hidden\" value=\"01/02/2021 00:00:00\">\n<input id=\"booking-appointment-time\" name=\"booking-appointment-time\" type=\"hidden\" value=\"14/12/2020 07:00:00\">\n<input id=\"booking-ReservedDate\" name=\"booking-ReservedDate\" type=\"hidden\" value=\"\">\n<input id=\"booking-ReservedTime\" name=\"booking-ReservedTime\" type=\"hidden\" value=\"14/12/2020 07:00:00\">\n<input id=\"booking-ReservedAppointmentId\" name=\"booking-ReservedAppointmentId\" type=\"hidden\" value=\"\">\n\n<h1 class=\"govuk-heading-l\">Check your booking</h1>\n<table class=\"govuk-table\">\n    <tbody><tr class=\"govuk-table__row\">\n        <th scope=\"row\" class=\"smbc-table__header\">Date:</th>\n        <td class=\"govuk-table__cell\">Monday 1 February 2021</td>\n    </tr>\n    <tr class=\"govuk-table__row\">\n        <th scope=\"row\" class=\"smbc-table__header\">Time:</th>\n        <td class=\"govuk-table__cell\">between 7am and 5:30pm</td>\n    </tr>\n</tbody></table>\n\n<button data-prevent-double-click=\"true\" data-disable-on-click=\"true\" class=\"govuk-button \" id=\"continue\" data-module=\"govuk-button\">\n    Submit\n</button>\n\n<input id=\"Path\" name=\"Path\" type=\"hidden\" value=\"page-three\"></form>\n                "
    },
    "Date selection": {
      "1": "\n    <fieldset class=\"govuk-fieldset\">\n\n    <input id=\"booking-ReservedDate\" name=\"booking-ReservedDate\" type=\"hidden\" value=\"\">\n    <input id=\"booking-ReservedTime\" name=\"booking-ReservedTime\" type=\"hidden\" value=\"14/12/2020 07:00:00\">\n    <input id=\"booking-ReservedAppointmentId\" name=\"booking-ReservedAppointmentId\" type=\"hidden\" value=\"\">\n<input id=\"booking-appointment-time\" name=\"booking-appointment-time\" type=\"hidden\" value=\"14/12/2020 07:00:00\">\n        <legend class=\"govuk-fieldset__legend govuk-fieldset__legend--l\">\n        <h1 class=\"govuk-fieldset__heading\">\n            Select a date\n\n        </h1>\n    </legend>\n\n\n    \n\n\n        \n    <div class=\"govuk-inset-text\">\n        You can select a date for UI Booking but you can not select a time. We’ll be with you between 7am and 5:30pm.\n    </div>\n\n\n        \n    <div class=\"govuk-inset-text\">\n        This is the next available appointment\n    </div>\n\n\n    \n\n    \n\n    \n<div class=\"smbc-calendar\">\n\n    \n<div class=\"smbc-calendar__header\">\n\n        <button type=\"button\" class=\"smbc-calendar__header__button smbc-calendar__header__button--disabled\" disabled=\"\"></button>\n\n    <p class=\"smbc-body smbc-calendar__header__text\">\n        February 2021\n    </p>\n\n        <button class=\"smbc-calendar__header__button\" name=\"BOOKING_MONTH_REQUEST\" value=\"01/03/2021 00:00:00\" formaction=\"/booking/ui-booking/page-three/month\">\n            <span class=\"smbc-calendar__header__chevron smbc-calendar__header__chevron--right\"></span>\n        </button>\n\n</div>\n\n\n    <table class=\"smbc-calendar__body\" summary=\"Appointment availability for February 2021\">\n        <tbody>\n\n            \n<tr>\n        <th scope=\"col\" class=\"smbc-calendar__weekdays\">\n\n            <p aria-hidden=\"true\" class=\"smbc-calendar__weekdays__text\">\n                M\n            </p>\n\n            <span class=\"govuk-visually-hidden\">Monday</span>\n\n        </th>\n        <th scope=\"col\" class=\"smbc-calendar__weekdays\">\n\n            <p aria-hidden=\"true\" class=\"smbc-calendar__weekdays__text\">\n                T\n            </p>\n\n            <span class=\"govuk-visually-hidden\">Tuesday</span>\n\n        </th>\n        <th scope=\"col\" class=\"smbc-calendar__weekdays\">\n\n            <p aria-hidden=\"true\" class=\"smbc-calendar__weekdays__text\">\n                W\n            </p>\n\n            <span class=\"govuk-visually-hidden\">Wednesday</span>\n\n        </th>\n        <th scope=\"col\" class=\"smbc-calendar__weekdays\">\n\n            <p aria-hidden=\"true\" class=\"smbc-calendar__weekdays__text\">\n                T\n            </p>\n\n            <span class=\"govuk-visually-hidden\">Thursday</span>\n\n        </th>\n        <th scope=\"col\" class=\"smbc-calendar__weekdays\">\n\n            <p aria-hidden=\"true\" class=\"smbc-calendar__weekdays__text\">\n                F\n            </p>\n\n            <span class=\"govuk-visually-hidden\">Friday</span>\n\n        </th>\n        <th scope=\"col\" class=\"smbc-calendar__weekdays\">\n\n            <p aria-hidden=\"true\" class=\"smbc-calendar__weekdays__text\">\n                S\n            </p>\n\n            <span class=\"govuk-visually-hidden\">Saturday</span>\n\n        </th>\n        <th scope=\"col\" class=\"smbc-calendar__weekdays\">\n\n            <p aria-hidden=\"true\" class=\"smbc-calendar__weekdays__text\">\n                S\n            </p>\n\n            <span class=\"govuk-visually-hidden\">Sunday</span>\n\n        </th>\n</tr>\n\n\n                <tr>\n                        \n    <td class=\"smbc-calendar__cell\">\n\n        <input type=\"radio\" class=\"smbc-calendar__input\" value=\"01/02/2021 00:00:00\" name=\"booking-appointment-date\" id=\"booking-appointment-date-1\" aria-describedby=\"booking-appointment-date-1-hint\">\n\n        <label aria-hidden=\"true\" class=\"smbc-calendar__label\" for=\"booking-appointment-date-1\">\n            1\n        </label>\n\n        <span class=\"govuk-visually-hidden\" id=\"booking-appointment-date-1-hint\">Monday 01 February</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">2</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">3</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">4</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">5</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">6</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">7</span>\n    </td>\n\n                </tr>\n                <tr>\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">8</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">9</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">10</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">11</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">12</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">13</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">14</span>\n    </td>\n\n                </tr>\n                <tr>\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">15</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">16</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">17</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">18</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">19</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">20</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">21</span>\n    </td>\n\n                </tr>\n                <tr>\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">22</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">23</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">24</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">25</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">26</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">27</span>\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n            <span aria-hidden=\"true\" class=\"smbc-calendar__label smbc-calendar__label--disabled\">28</span>\n    </td>\n\n                </tr>\n                <tr>\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                </tr>\n                <tr>\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                        \n    <td class=\"smbc-calendar__cell smbc-calendar__cell--disabled\">\n    </td>\n\n                </tr>\n        </tbody>\n\n    </table>\n\n</div>\n\n    </fieldset>\n"
    }
  },
  "__version": "6.1.0"
}
