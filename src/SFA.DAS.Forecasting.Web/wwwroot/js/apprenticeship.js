function ApprenticeshipForm() {
    this.apprenticeshipRoleSelectId = 'choose-apprenticeship'
    this.apprenticeshipRole = document.getElementById(this.apprenticeshipRoleSelectId)
    this.selectedStandardId = 0
}

ApprenticeshipForm.prototype.init = function() {
    this.autoComplete()
}


ApprenticeshipForm.prototype.autoComplete = function() {
    var that = this
    accessibleAutocomplete.enhanceSelectElement({
        selectElement: that.apprenticeshipRole,
        minLength: 2,
        autoselect: false,
        defaultValue: '',
        displayMenu: 'overlay',
        placeholder: '',
        showAllValues: true,
        onConfirm: function (opt) {
            var txtInput = document.querySelector('#' + that.apprenticeshipRoleSelectId);
            var searchString = opt || txtInput.value;
            var requestedOption = [].filter.call(this.selectElement.options,
                function (option) {
                return (option.textContent || option.innerText) === searchString
                }
            )[0];
            if (requestedOption) {
                requestedOption.selected = true;
                if (requestedOption.value) {
                that.selectedStandardId = requestedOption.value
                } else {
                that.selectedStandardId = 0
                }
            } else {
                this.selectElement.selectedIndex = 0;
                that.selectedStandardId = 0
            }
        }
    });
}

var apprenticeshipForm = new ApprenticeshipForm();
apprenticeshipForm.init()