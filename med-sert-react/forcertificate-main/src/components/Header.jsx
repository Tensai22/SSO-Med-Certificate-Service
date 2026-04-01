import '../css/Header.css'

function Header(){
    return(
        <div className="cab_header_center">
            <div className="title_text_block">

                <div ng-if="$ctrl.byArray">
                    <div className="top_text">


                        <div className="menu">
                            <a href="//sso.satbayev.university" className="menu_link">
                                <div className="menu_button">
                                    <img src="/assets/home.png" alt="Домой" width="18" height="18" />
                                </div>
                            </a>
                        </div>



                    </div>
                </div>
            </div>
            <div className="operator_block">
                <div className="visually_impaired">
                    <a href="#" className="bvi-open" style={{}}><i className="icon-eye"></i>&nbsp;&nbsp; Версия для
                        слабовидящих</a>
                </div>
                <div className="oper_name_block">
                    <p>Нариман Б. Н.</p>
                    <p className="stat">Бакалавриат</p>

                </div>
                <div className="oper_set_block">

                    <div className="img default-img" ng-if="$ctrl.imageSrc &amp;&amp; $ctrl.showImage"
                         ng-class="{'default-img': !$ctrl.hasImage}">
                        <img ng-src="/assets/default-avatar.png"
                             src="/assets/default-avatar.png"
                             alt="Аватар"
                             className="bvi-img"/>

                    </div>

                    <div className="exit_block" ng-click="$ctrl.logOut()">
                        <i className="icon-enter"></i>
                        <p>Выход</p>
                    </div>
                    <div className="lang_block">
                        <button ng-click="$ctrl.changeLanguage('kz')" className="button oper_button"
                                ng-class="{active: $ctrl.isActiveLanguage('kz')}">
                            Қаз
                        </button>
                        <button ng-click="$ctrl.changeLanguage('ru')" className="button oper_button active"
                                ng-class="{active: $ctrl.isActiveLanguage('ru')}">
                            Рус
                        </button>
                        <button ng-click="$ctrl.changeLanguage('en')" className="button oper_button"
                                ng-class="{active: $ctrl.isActiveLanguage('en')}">
                            Eng
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Header;