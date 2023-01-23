function ShowFundingEstimate() {
    this.apprenticeshipRoleSelectId = 'choose-apprenticeship'
    this.apprenticeshipRole = document.getElementById(this.apprenticeshipRoleSelectId)
    this.apprenticeshipNumber = document.getElementById('no-of-app')
    this.apprenticeshipDateMonth = document.getElementById('startDateMonth')
    this.apprenticeshipDateYear = document.getElementById('startDateYear')
    this.selectedStandardId = 0
    this.numberOfApprentices = 0
    this.startMonth = 0
    this.startYear = 0
    this.monthsRemaining = 0
    this.estimate = 0
    this.showEstimate = false
}

ShowFundingEstimate.prototype.init = function() {
    this.autoComplete()
    this.setupEvents()
    this.pageLoad()
    this.checkFieldValues()
}

ShowFundingEstimate.prototype.pageLoad = function () {
    if (this.apprenticeshipRole.value !== "") {
      this.selectedStandardId = this.apprenticeshipRole.value
    }
    if (this.apprenticeshipNumber.value !== "") {
      this.numberOfApprentices = this.apprenticeshipNumber.value
    }
    if (this.apprenticeshipDateMonth.value !== "") {
      this.startMonth = this.apprenticeshipDateMonth.value
      this.dateChange()
    }
    if (this.apprenticeshipDateYear.value !== "") {
      this.startYear = this.apprenticeshipDateYear.value
      this.dateChange()
    }
  }


ShowFundingEstimate.prototype.setupEvents = function () {
    var that = this
    this.apprenticeshipNumber.onkeyup = function () {
        that.numberOfApprentices = this.value | 0
        that.checkFieldValues()
    }
    this.apprenticeshipDateMonth.onkeyup = function () {
        that.startMonth = this.value | 0
        that.dateChange()
        that.checkFieldValues()
    }
    this.apprenticeshipDateYear.onkeyup = function () {
        that.startYear = this.value | 0
        that.dateChange()
        that.checkFieldValues()
    }
}

ShowFundingEstimate.prototype.dateChange = function () {
    var futureDate = new Date(this.startYear + ", " + this.startMonth + ", 1")
    this.monthsRemaining = this.calculateMonthsDifference(futureDate);
}


ShowFundingEstimate.prototype.autoComplete = function() {
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
            that.checkFieldValues()
        }
    });
}

ShowFundingEstimate.prototype.getEstimate = function () {
    var that = this
    var params = 'SelectedStandardId=' + this.selectedStandardId + '&NumberOfApprentices=' + this.numberOfApprentices + '&StartDate=' + this.startYear + '-' + this.startMonth + '-01'
    var url = window.location.href.split('?')[0] + '/funding-estimate?' + params
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState === 4) {
        try {
            var response = JSON.parse(xhr.responseText)
            var estimate = parseInt(response.amount)
            if (isNaN(estimate)) {
            that.estimate = 0
            that.showEstimate = false
            } else {
            that.estimate = estimate
            that.hasEnoughFunding = response.hasEnoughFunding
            that.showEstimate = true
            }
            that.updateUI()
        } catch (e) {
            that.showEstimate = false
            that.updateUI()
        }
        }
    }
    xhr.open("GET", url, true);
    xhr.send();
}

ShowFundingEstimate.prototype.calculateMonthsDifference = function(futureDate) {
    var date = new Date();
    var firstOfCurrentMonth = new Date(date.getFullYear(), date.getMonth(), 1);
    var difference = (futureDate.getTime() - firstOfCurrentMonth.getTime()) / 1000;
    difference = difference / (60 * 60 * 24 * 7 * 4);
    return Math.round(difference) + 1;
  }
  
ShowFundingEstimate.prototype.checkFieldValues = function () {
    if (this.selectedStandardId.length > 0 && this.numberOfApprentices > 0 && this.monthsRemaining > 0) {
      this.showEstimate = true
      this.getEstimate()
    } else {
      this.showEstimate = false
      this.updateUI()
    }
}
  
ShowFundingEstimate.prototype.updateUI = function () {

    var infoPanel = document.getElementById('details-about-funding');
    var estimatePanel = document.getElementById('details-about-funding-calculated');

    if (this.showEstimate) {

      //document.getElementById('field-estimate').innerText = this.estimate.toLocaleString()
      infoPanel.style.display = 'none'
      estimatePanel.style.display = 'block'

    } else {
      infoPanel.style.display = 'block'
      estimatePanel.style.display = 'none'
    }
}

var showFundingEstimate = new ShowFundingEstimate().init()